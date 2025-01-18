namespace SecretSwitcher.GenerateMongoSecrets
{
    public class GenerateCurrentMongoSecretsRequest
    {
        public string BaseAddress { get; set; } = null!;
        public string? Environment { get; set; }
    }
}