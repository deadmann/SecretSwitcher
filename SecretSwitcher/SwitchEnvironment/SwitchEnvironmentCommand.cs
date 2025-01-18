using SecretSwitcher.Common;

namespace SecretSwitcher.SwitchEnvironment;

internal class SwitchEnvironmentCommand : ICommand<SwitchEnvironmentRequest>
{
    public Task ExecuteAsync(SwitchEnvironmentRequest environmentRequest)
    {
        if (string.IsNullOrEmpty(environmentRequest.Environment))
        {
            Console.WriteLine("Environment value is missing. Please provide an environment.");
            return Task.CompletedTask;
        }

        var projectInfos = SwitchEnvironmentCommandOperations.TraverseProjectsAndRetrieveSecrets(environmentRequest.BaseAddress);
        SwitchEnvironmentCommandOperations.DisplayProjectSecrets(projectInfos, environmentRequest.Environment);
        return Task.CompletedTask;
    }
}