using SecretSwitcher.Common;
using SecretSwitcher.Data;
using TextCopy;

namespace SecretSwitcher.SetDbSecretsFromClipboard;

public class SetSecretsCommand : ICommand<SetSecretsRequest>
{
    private readonly UserSecretsService _userSecretsService;

    public SetSecretsCommand()
    {
        var databaseContext = new DatabaseContext(ConfigManager.GetMongoDbConnectionString()!, ConfigManager.GetDatabaseName());
        var userSecretsRepository = new UserSecretsRepository(databaseContext.Database);
        _userSecretsService = new UserSecretsService(userSecretsRepository);
    }
    
    public async Task ExecuteAsync(SetSecretsRequest request)
    {
        if (string.IsNullOrEmpty(request.Environment))
        {
            Console.WriteLine("Environment value is missing. Please provide an environment.");
            return;
        }
        
        if (string.IsNullOrEmpty(request.ProjectName))
        {
            Console.WriteLine("Project Name value is missing. Please provide a project name.");
            return;
        }
        
        var projectInfos = ProjectManager.TraverseAllProjects(request.BaseAddress);
        var matchingProjects = projectInfos
            .Where(p => p.Name.Contains(request.ProjectName, StringComparison.OrdinalIgnoreCase))
            .Select((s, i) => new { Project = s, Index = i })
            .ToList();

        if (!matchingProjects.Any())
        {
            Console.WriteLine("No projects found.");
            return;
        }

        if (matchingProjects.Count > 1)
        {
            Console.WriteLine("Multiple projects found:");
            foreach (var each in matchingProjects)
            {
                Console.WriteLine($"{each.Index+1}. {each.Project.Name} (ID: {each.Project.SecretId})");
            }

            Console.WriteLine("Please select a project or press Enter to cancel.");
            var projectRowInput = Console.ReadLine();
            if (!int.TryParse(projectRowInput, out var projectRow))
            {
                Console.WriteLine("Operation canceled.");
                return;
            }
            
            var projectIndex = projectRow - 1;

            var selectedProject = matchingProjects.FirstOrDefault(p => p.Index == projectIndex);
            if (selectedProject != null)
            {
                await SelectEnvironmentForSet(request, selectedProject.Project);
            }
            else
            {
                Console.WriteLine("Invalid selection or operation canceled.");
            }
        }
        else
        {
            var project = matchingProjects.First().Project;
            var isConfirmed = AskConfirmation($"Did you mean '{project.Name}'? (y/n)");

            if (isConfirmed)
            {
                await SelectEnvironmentForSet(request, project);
            }
            else
            {
                Console.WriteLine("Operation canceled.");
            }
        }
    }

    private async Task SelectEnvironmentForSet(SetSecretsRequest request, ProjectInfo project)
    {
        if (project.ProjectType is not ProjectType.Executable and not ProjectType.WebApp)
        {
            Console.WriteLine("Project type is neither Executable nor WebApp. Operation cannot be processed.");
            return;
        }
        
        var clipboardContent = await GetClipboardContent();

        if (string.IsNullOrEmpty(clipboardContent))
        {
            Console.WriteLine("Clipboard content is empty.");
            return;
        }
        
        Console.WriteLine("Content Found in the Clipboard:");
        ColorHelper.PrintInfo(clipboardContent);
        
        Console.WriteLine($"You are about to set secrets for the environment '{request.Environment}'.");
        if (AskConfirmation("Do you want to override the existing data?"))
        {
            // Console.WriteLine("===> Replacing the old content with new clipboard data (place a placeholder for comparison).");
            await SetSecretsInDb(project, request.Environment!, clipboardContent);
            Console.WriteLine("Secrets successfully set.");
        }
        else
        {
            Console.WriteLine("Operation canceled.");
        }
    }

    private static async Task<string?> GetClipboardContent()
    {
        ColorHelper.PrintWarning("Is the content in the clipboard ready? Press Enter if yes, otherwise copy it first.");

        // Wait for the user to press enter or copy data to clipboard
        Console.ReadLine();
        var clipboard = new Clipboard();
        return await clipboard.GetTextAsync();
    }

    private static bool AskConfirmation(string message)
    {
        Console.WriteLine(message);
        var response = Console.ReadLine()?.Trim().ToLower();
        return response == "y" || response == "yes";
    }

    private async Task SetSecretsInDb(ProjectInfo project, string environment, string? content)
    {
        Console.WriteLine($"Setting secrets for project: {project.Name} in environment: {environment}...");

        if (string.IsNullOrEmpty(content))
        {
            Console.WriteLine("No content provided for the secret. Skipping update.");
            return;
        }
        
        // Add the secret for the given project and environment
        await _userSecretsService.AddNewSecretAsync(
            projectName: project.Name,
            secretId: project.SecretId, // Assuming this is provided in the `ProjectInfo`
            environment: environment,
            content: content
        );

        Console.WriteLine($"Secrets set successfully for project: {project.Name} in environment: {environment}.");
    }
}