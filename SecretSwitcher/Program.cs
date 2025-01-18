using SecretSwitcher.Common;

namespace SecretSwitcher;

class Program
{
    static void Main(string[] args)
    {
        // Ensure user has administrative privileges
        if (!AdminUtility.IsUserAdmin())
        {
            Console.WriteLine("This program requires administrative privileges. Please run it as an administrator.");
            return;
        }

        // Load base address configuration from the environment file
        string? baseAddress = ConfigManager.LoadBaseAddress();
        if (string.IsNullOrEmpty(baseAddress))
        {
            Console.WriteLine("Base address not configured. Please check the environment file.");
            return;
        }

        // Execute command if arguments exist, otherwise start the interactive session
        if (args.Length > 0)
        {
            CliManager.ExecuteCommand(args, baseAddress);
        }
        else
        {
            CliManager.StartInteractiveSession(baseAddress);
        }
    }
}