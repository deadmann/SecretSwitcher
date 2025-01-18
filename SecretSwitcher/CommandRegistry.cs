using SecretSwitcher.Common;
using SecretSwitcher.GenerateEmptyMongoSecrets;
using SecretSwitcher.GenerateMongoSecrets;
using SecretSwitcher.RemoveSecretsByEnvironment;
using SecretSwitcher.SwitchEnvironment;

namespace SecretSwitcher;

internal static class CommandRegistry
{
    public const string SwitchEnvironment = "switch";
    public const string GenerateCurrentMongoSecrets = "gcms";
    public const string GenerateEmptyMongoSecrets = "gems";
    public const string RemoveSecrets = "remove";
    
    private static readonly Dictionary<string, (object Command, Type RequestType)> CommandDictionary = new()
    {
        { SwitchEnvironment, (new SwitchEnvironmentCommand(), typeof(SwitchEnvironmentRequest)) },
        { GenerateCurrentMongoSecrets, (new GenerateCurrentMongoSecretsCommand(), typeof(GenerateCurrentMongoSecretsRequest)) },
        { GenerateEmptyMongoSecrets, (new GenerateEmptyMongoSecretsCommand(), typeof(GenerateEmptyMongoSecretsRequest)) },
        { RemoveSecrets, (new RemoveSecretsByEnvironmentCommand(), typeof(RemoveSecretsByEnvironmentRequest))}
    };

    public static (object Command, object Request)? GetCommandWithRequest(
        string commandName, 
        string[] args, 
        string baseAddress)
    {
        if (!CommandDictionary.TryGetValue(commandName, out var commandData)) 
        {
            return null;
        }

        var (command, requestType) = commandData;
        var request = CreateRequest(commandName, requestType, args, baseAddress);
        return (command, request);
    }

    private static object CreateRequest(string commandName, Type requestType, string[] args, string baseAddress)
    {
        return commandName switch
        {
            SwitchEnvironment when requestType == typeof(SwitchEnvironmentRequest) =>
                new SwitchEnvironmentRequest
                {
                    BaseAddress = baseAddress, 
                    Environment = args.Length > 0 ? args[0] : null
                },
            GenerateCurrentMongoSecrets when requestType == typeof(GenerateCurrentMongoSecretsRequest) =>
                new GenerateCurrentMongoSecretsRequest
                {
                    BaseAddress = baseAddress, 
                    Environment = args.Length > 0 ? args[0] : null 
                },
            GenerateEmptyMongoSecrets when requestType == typeof(GenerateEmptyMongoSecretsRequest) =>
                new GenerateEmptyMongoSecretsRequest
                {
                    BaseAddress = baseAddress,
                    Environment = args.Length > 0 ? args[0] : null
                },
            RemoveSecrets when requestType == typeof(RemoveSecretsByEnvironmentRequest) =>
                new RemoveSecretsByEnvironmentRequest
                {
                    Environment = args.Length >0? args[0] : null
                },
            
            _ => throw new InvalidOperationException($"Unknown request type for command '{commandName}'.")
        };
    }
    
    public static void ExecuteCommandMenu<TRequest>(string commandName, TRequest request)
    {
        if (CommandDictionary.TryGetValue(commandName, out var command) && command.Command is ICommand<TRequest> executableCommand)
        {
            Console.Clear();
            
            // Execute the command safely
            executableCommand.ExecuteAsync(request)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        else
        {
            // Handle the case where the command or its type is not found
            throw new InvalidOperationException($"Command '{commandName}' is not valid or does not support the given request type.");
        }
    }

    public static void ExecuteCommandCli((object Command, object Request)? commandAndRequest)
    {
        if (commandAndRequest.HasValue)
        {
            var (command, request) = commandAndRequest.Value;
        
            // Check if command implements ICommand<TRequest> where TRequest is the runtime type of request
            var commandType = command.GetType();
            var requestType = request.GetType();
        
            // Try to find the Execute method with parameter of the request type
            var executeMethod = commandType.GetMethod("ExecuteAsync", new[] { requestType });

            if (executeMethod != null)
            {
                // Invoke the Execute method dynamically
                var task = executeMethod.Invoke(command, new[] { request });
                (task as Task)!
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
                
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            else
            {
                // Handle the case where the Execute method doesn't exist
                throw new InvalidOperationException($"Command '{commandType.FullName}' does not have an Execute method that accepts a parameter of type '{requestType.FullName}'.");
            }
        }
        else
        {
            // Handle case where commandAndRequest is null
            throw new ArgumentNullException(nameof(commandAndRequest), "Command and request cannot be null.");
        }
    }
}