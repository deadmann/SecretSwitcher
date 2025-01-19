namespace SecretSwitcher.Common.Extensions;

public static class StringExtensions
{
    public static string ReplacePlaceholdersWithValues(this string content, string environment)
    {
        const string placeholderPattern = @"\{\{([A-Za-z0-9_]+)\}\}"; // Matches placeholders like {{DATA__FOO}}
        return System.Text.RegularExpressions.Regex.Replace(content, placeholderPattern, match =>
        {
            var key = match.Groups[1].Value; // Extract the key inside {{ }}
        
            // Fetch value for the key, considering the environment
            var value = ConfigManager.GetDataConfigValue(key, environment);

            if (value == null)
            {
                Console.WriteLine($"  Warning: No value found for placeholder '{key}'. Placeholder left unchanged.");
                return match.Value; // Return the original placeholder if no value is found
            }

            return value;
        });
    }
}