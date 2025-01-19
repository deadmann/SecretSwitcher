namespace SecretSwitcher.SetDbSecretsFromClipboard;

public class SetSecretsRequest
{
    public string BaseAddress { get; set; } = null!;
    public string? Environment { get; set; }
    public string? ProjectName { get; set; }
}