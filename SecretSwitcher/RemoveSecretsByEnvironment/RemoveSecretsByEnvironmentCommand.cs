using SecretSwitcher.Common;
using SecretSwitcher.Data;

namespace SecretSwitcher.RemoveSecretsByEnvironment;

internal class RemoveSecretsByEnvironmentCommand : ICommand<RemoveSecretsByEnvironmentRequest>
{
    private readonly UserSecretsService _userSecretsService;

    public RemoveSecretsByEnvironmentCommand()
    {
        var databaseContext = new DatabaseContext(ConfigManager.GetMongoDbConnectionString()!, ConfigManager.GetDatabaseName());
        var userSecretsRepository = new UserSecretsRepository(databaseContext.Database);
        _userSecretsService = new UserSecretsService(userSecretsRepository);
    }

    public async Task ExecuteAsync(RemoveSecretsByEnvironmentRequest request)
    {
        if (string.IsNullOrEmpty(request.Environment))
        {
            Console.WriteLine("Environment value is missing. Please provide an environment.");
            return;
        }

        Console.WriteLine($"Are you sure you want to delete all secrets for environment '{request.Environment}'? (yes/no)");
        var confirmation = Console.ReadLine()?.Trim().ToLower();

        if (confirmation != "yes")
        {
            Console.WriteLine("Operation canceled.");
            return;
        }

        Console.WriteLine($"To confirm deletion, please type the environment name: '{request.Environment}'");
        var secondConfirmation = Console.ReadLine()?.Trim();

        if (secondConfirmation != request.Environment)
        {
            Console.WriteLine("Environment names do not match. Operation aborted.");
            return;
        }

        var secrets = await _userSecretsService.FilterSecretsByEnvironmentAsync(request.Environment);

        if (secrets.Count == 0)
        {
            Console.WriteLine($"No secrets found for environment '{request.Environment}'.");
            return;
        }

        Console.WriteLine($"Found {secrets.Count} secrets for environment '{request.Environment}'. Deleting...");

        foreach (var secret in secrets)
        {
            await _userSecretsService.RemoveSecretByIdAsync(secret.Id);
            Console.WriteLine($"  Deleted secret for project '{secret.ProjectName}' with SecretId '{secret.SecretId}'.");
        }

        Console.WriteLine($"All secrets for environment '{request.Environment}' have been successfully deleted.");
    }
}
