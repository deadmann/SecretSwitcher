namespace SecretSwitcher.Common;

public static class ProjectPaths
{
    public static string GetUserSecretsPath(string? userSecretsId)
    {
        // Get the user profile directory in a cross-platform way
        string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // Build the user secrets path dynamically based on the platform
        string baseUserSecretsPath = Path.Combine(userProfile, "AppData", "Roaming", "Microsoft", "UserSecrets", userSecretsId);
            
        // We will look for the secrets.json file under this path
        return Path.Combine(baseUserSecretsPath, "secrets.json");
    }
}