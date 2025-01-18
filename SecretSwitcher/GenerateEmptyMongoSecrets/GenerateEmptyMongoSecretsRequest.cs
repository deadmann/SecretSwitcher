namespace SecretSwitcher.GenerateEmptyMongoSecrets
{
    public class GenerateEmptyMongoSecretsRequest
    {
        public string BaseAddress { get; set; } = null!;
        public string? Environment { get; set; }
    }
}