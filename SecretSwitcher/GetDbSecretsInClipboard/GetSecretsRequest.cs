namespace SecretSwitcher.GetDbSecretsInClipboard;

public class GetSecretsRequest
{
    public string BaseAddress { get; set; } = null!;
    public string? Environment { get; set; }
    public string? ProjectName { get; set; }
}