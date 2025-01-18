using SecretSwitcher.Common;
using SecretSwitcher.Data;

namespace SecretSwitcher.SwitchEnvironment;

internal static class SwitchEnvironmentCommandOperations
{
    public static List<ProjectInfo> TraverseProjectsAndRetrieveSecrets(string baseAddress)
    {
        return ProjectManager.TraverseProjectsAndRetrieveSecrets(baseAddress);
    }

    public static void DisplayProjectSecrets(List<ProjectInfo> projectInfos, string environment)
    {
        // Initialize the UserSecretsService with necessary dependencies
        var databaseContext = new DatabaseContext(ConfigManager.GetMongoDbConnectionString()!, ConfigManager.GetDatabaseName());
        var userSecretsRepository = new UserSecretsRepository(databaseContext.Database);
        var userSecretsService = new UserSecretsService(userSecretsRepository);
        
        Console.WriteLine("\n===== Project Secrets =====");
        foreach (var project in projectInfos)
        {
            Console.WriteLine($"Project: {project.Name}");
                
            if (string.IsNullOrEmpty(project.SecretId))
            {
                Console.WriteLine("  Warning: No UserSecretsId found.");
            }
            else
            {
                // Get the secret file path from the new class
                string secretFile = ProjectPaths.GetUserSecretsPath(project.SecretId);

                // Check if the secret file exists and display the appropriate message
                Console.WriteLine(File.Exists(secretFile)
                    ? $"  Secrets: {secretFile}"
                    : $"  Warning: Secret file not found for UserSecretsId {project.SecretId}");
                
                // Check the database for the corresponding secret document
                var secretsInDb = userSecretsService
                    .FilterSecretsByEnvironmentAsync(environment)
                    .Result
                    .FirstOrDefault(secret => secret.SecretId == project.SecretId);

                if (secretsInDb != null)
                {
                    Console.WriteLine($"  Database: Secret exists in the database for Environment '{environment}'.");

                    // Replace the file content with content from the database
                    try
                    {
                        // Ensure the directory exists before creating the file
                        string directory = Path.GetDirectoryName(secretFile) ?? throw new InvalidOperationException("Invalid file path.");

                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                            Console.WriteLine($"  Created missing directory: {directory}");
                        }
                        
                        using (var fileStream = new FileStream(secretFile, FileMode.Create, FileAccess.Write, FileShare.None))
                        using (var writer = new StreamWriter(fileStream))
                        {
                            writer.Write(secretsInDb.Content);
                        }

                        Console.WriteLine($"  Secrets updated: File replaced with content from the database for '{project.Name}'.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Error: Failed to update the secret file for '{project.Name}'. Details: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"  Warning: No database entry found for UserSecretsId {project.SecretId} in Environment '{environment}'.");
                }
            }
        }

        Console.WriteLine($"\nEnvironment switched to '{environment}'.");
    }
}