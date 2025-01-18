using SecretSwitcher.Common;
using SecretSwitcher.Data;
using SecretSwitcher.SwitchEnvironment;

namespace SecretSwitcher.GenerateEmptyMongoSecrets;

internal class GenerateEmptyMongoSecretsCommand : ICommand<GenerateEmptyMongoSecretsRequest>
{
    private readonly UserSecretsService _userSecretsService;

    public GenerateEmptyMongoSecretsCommand()
    {
        var databaseContext = new DatabaseContext(ConfigManager.GetMongoDbConnectionString()!, ConfigManager.GetDatabaseName());
        var userSecretsRepository = new UserSecretsRepository(databaseContext.Database);
        _userSecretsService = new UserSecretsService(userSecretsRepository);
    }

    public async Task ExecuteAsync(GenerateEmptyMongoSecretsRequest request)
    {
        if (string.IsNullOrEmpty(request.Environment))
        {
            Console.WriteLine("Environment value is missing. Please provide an environment.");
            return;
        }

        // Get all projects and retrieve their secrets
        var projectInfos = SwitchEnvironmentCommandOperations.TraverseProjectsAndRetrieveSecrets(request.BaseAddress);
            
        foreach (var project in projectInfos)
        {
            if (string.IsNullOrEmpty(project.SecretId))
            {
                Console.WriteLine($"  Warning: Project '{project.Name}' has no SecretId.");
                continue;  // Skip projects without a secret id
            }
            
            // Check if the document already exists in MongoDB
            var existingSecrets = await _userSecretsService.FilterSecretsByEnvironmentAsync(request.Environment);
            var existingSecret = existingSecrets
                .FirstOrDefault(s => s.SecretId == project.SecretId && s.Environment == request.Environment);

            if (existingSecret == null)
            {
                // No document exists, so we can add a new one
                await _userSecretsService.AddNewSecretAsync(
                    project.Name,
                    project.SecretId,
                    request.Environment,
                    $"{{{Environment.NewLine}}}");

                Console.WriteLine($"  Created Mongo document for project: {project.Name} in environment: {request.Environment}");
            }
            else
            {
                Console.WriteLine($"  Mongo document already exists for project: {project.Name} in environment: {request.Environment}.");
            }
        }
    }
}