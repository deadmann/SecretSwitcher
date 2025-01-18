namespace SecretSwitcher.SwitchEnvironment;

internal class SwitchEnvironmentRequest
{
    public string BaseAddress { get; set; } = null!;
    public string? Environment { get; set; }
}