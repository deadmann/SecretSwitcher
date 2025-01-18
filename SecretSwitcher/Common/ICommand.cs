namespace SecretSwitcher.Common;

public interface ICommand<in TRequest>
{
    Task ExecuteAsync(TRequest request);
}