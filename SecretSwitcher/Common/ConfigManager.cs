namespace SecretSwitcher.Common;

internal static class ConfigManager
{
    private const string ConfigFilePath = ".env";

    // Loads a key-value pair from the .env file
    private static string? GetConfigValue(string key)
    {
        if (File.Exists(ConfigFilePath))
        {
            foreach (var line in File.ReadLines(ConfigFilePath))
            {
                if (line.StartsWith(key))
                {
                    var skipLength = key.Length + "=".Length;
                    return line[skipLength..].Trim();
                }
            }
        }

        return null;
    }

    // Retrieves a key specific to the environment or defaults if not found
    public static string? GetDataConfigValue(string key, string? environment = null)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        }

        // Generate environment-specific key if environment is provided
        if (!string.IsNullOrWhiteSpace(environment))
        {
            var envSpecificKey = $"DATA_{environment}__{key}";
            var envValue = GetConfigValue(envSpecificKey);
            if (envValue != null) return envValue;
        }

        // Fallback to the default key
        var defaultKey = $"DATA__{key}";
        return GetConfigValue(defaultKey);
    }
    
    // Retrieves the BaseAddress from the .env configuration
    public static string? LoadBaseAddress() => GetConfigValue("SWITCHER_BASE_ADDRESS");

    // Retrieves the connection string from the .env configuration
    public static string? GetMongoDbConnectionString() => GetConfigValue("SWITCHER_MONGODB_CONNECTION_STRING");

    // Retrieves the database name from the .env configuration
    public static string? GetDatabaseName() => GetConfigValue("SWITCHER_DATABASE_NAME");
}