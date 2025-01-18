using SecretSwitcher.GenerateEmptyMongoSecrets;
using SecretSwitcher.GenerateMongoSecrets;
using SecretSwitcher.RemoveSecretsByEnvironment;
using SecretSwitcher.SwitchEnvironment;

namespace SecretSwitcher;

internal static class CliManager
{
    public static void StartInteractiveSession(string baseAddress)
    {
        while (true)
        {
            Console.Clear();
            
            Console.WriteLine("\n===== Menu =====");
            Console.WriteLine("1. Switch Environment (switch)");
            Console.WriteLine("2. Generate Mongo Secrets Documents from Current Setup (gcms)");
            Console.WriteLine("3. Generate Empty Mongo Secrets Documents for Projects (gems)");
            Console.WriteLine("4. Remove All Secrets for an Environment (remove)");
            Console.WriteLine("0. Quit");
            Console.Write("Select an option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                case CommandRegistry.SwitchEnvironment:
                {
                    Console.Write("Enter environment value: ");
                    var environment = Console.ReadLine();
                    CommandRegistry.ExecuteCommandMenu(CommandRegistry.SwitchEnvironment,
                        new SwitchEnvironmentRequest { Environment = environment, BaseAddress = baseAddress });
                }
                    break;

                case "2":
                case CommandRegistry.GenerateCurrentMongoSecrets:
                {
                    Console.Write("Enter environment value: ");
                    var environment = Console.ReadLine();
                    CommandRegistry.ExecuteCommandMenu(CommandRegistry.GenerateCurrentMongoSecrets,
                        new GenerateCurrentMongoSecretsRequest { Environment = environment, BaseAddress = baseAddress });
                }
                    break;

                case "3":
                case CommandRegistry.GenerateEmptyMongoSecrets:
                {
                    Console.Write("Enter environment value: ");
                    var environment = Console.ReadLine();
                    CommandRegistry.ExecuteCommandMenu(CommandRegistry.GenerateEmptyMongoSecrets,
                        new GenerateEmptyMongoSecretsRequest { Environment = environment, BaseAddress = baseAddress });
                }
                    break;

                case "4":
                case CommandRegistry.RemoveSecrets:
                {
                    Console.Write("Enter environment value: ");
                    var environment = Console.ReadLine();
                    CommandRegistry.ExecuteCommandMenu(CommandRegistry.RemoveSecrets,
                        new RemoveSecretsByEnvironmentRequest{ Environment = environment });
                }
                    break;
                
                case "0":
                    Console.WriteLine("Exiting...");
                    return;

                default:
                    Console.WriteLine("Invalid selection. Please try again.");
                    break;
            }
        }
    }

    public static void ExecuteCommand(string[] args, string baseAddress)
    {
        string commandName = args[0].ToLower();
        string[] commandArgs = args.Length > 1 ? args[1..] : Array.Empty<string>();
        var command = CommandRegistry.GetCommandWithRequest(commandName, commandArgs, baseAddress);
        CommandRegistry.ExecuteCommandCli(command);
    }
}