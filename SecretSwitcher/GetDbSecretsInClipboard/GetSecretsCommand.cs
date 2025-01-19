using SecretSwitcher.Common;
using SecretSwitcher.Common.Extensions;
using SecretSwitcher.Data;
using TextCopy;

namespace SecretSwitcher.GetDbSecretsInClipboard;

public class GetSecretsCommand : ICommand<GetSecretsRequest>
{
    private readonly UserSecretsService _userSecretsService;

    public GetSecretsCommand()
    {
        var databaseContext = new DatabaseContext(ConfigManager.GetMongoDbConnectionString()!, ConfigManager.GetDatabaseName());
        var userSecretsRepository = new UserSecretsRepository(databaseContext.Database);
        _userSecretsService = new UserSecretsService(userSecretsRepository);
    }
    
    public async Task ExecuteAsync(GetSecretsRequest request)
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
                await SelectEnvironmentForGet(request, selectedProject.Project);
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
                await SelectEnvironmentForGet(request, project);
            }
            else
            {
                Console.WriteLine("Operation canceled.");
            }
        }
    }

    private async Task SelectEnvironmentForGet(GetSecretsRequest request, ProjectInfo project)
    {
        if (project.ProjectType is not ProjectType.Executable and not ProjectType.WebApp)
        {
            Console.WriteLine("Project type is neither Executable nor WebApp. Operation cannot be processed.");
            return;
        }
        
        var shouldProcessContent = AskConfirmation("Should we process the content (replace placeholders)?");
            
        // Fetch the secret content
        var content = await GetSecretsFromDb(project, request.Environment!);

        if (shouldProcessContent)
        {
            content = content.ReplacePlaceholdersWithValues(request.Environment!);
        }

        var clipboard = new Clipboard();
        await clipboard.SetTextAsync(content);
        // Place the content into clipboard
        Console.WriteLine("The content fetched is placed in your clipboard.");
    }

    private static bool AskConfirmation(string message)
    {
        Console.WriteLine(message);
        var response = Console.ReadLine()?.Trim().ToLower();
        return response == "y" || response == "yes";
    }

    private async Task<string> GetSecretsFromDb(ProjectInfo project, string environment)
    {
        // Use the UserSecretsService to fetch secrets by project name and environment
        Console.WriteLine($"Fetching secrets for project: {project.Name} in environment: {environment}...");

        var secrets = await _userSecretsService.FilterSecretsByProjectNameAsync(project.Name);

        // Filter by environment
        var filteredSecret = secrets.FirstOrDefault(s => s.Environment == environment);

        if (filteredSecret != null)
        {
            Console.WriteLine($"Fetched secret version: {filteredSecret.Version} created at {filteredSecret.CreatedAt}");
            return filteredSecret.Content; // Return the content of the retrieved secret
        }

        Console.WriteLine("No secrets found for the specified project and environment.");
        return string.Empty;
    }
}