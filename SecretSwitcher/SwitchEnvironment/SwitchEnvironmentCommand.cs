using SecretSwitcher.Common;

namespace SecretSwitcher.SwitchEnvironment;

internal class SwitchEnvironmentCommand : ICommand<SwitchEnvironmentRequest>
{
    public Task ExecuteAsync(SwitchEnvironmentRequest request)
    {
        if (string.IsNullOrEmpty(request.Environment))
        {
            Console.WriteLine("Environment value is missing. Please provide an environment.");
            return Task.CompletedTask;
        }

        var projectInfos = SwitchEnvironmentCommandOperations.TraverseProjectsAndRetrieveSecrets(request.BaseAddress);
        SwitchEnvironmentCommandOperations.DisplayProjectSecrets(projectInfos, request.Environment);
        return Task.CompletedTask;
    }
}