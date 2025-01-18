namespace SecretSwitcher.Data;

public class UserSecretsService
{
    private readonly UserSecretsRepository _repository;

    public UserSecretsService(UserSecretsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task AddNewSecretAsync(string projectName, string? secretId, string environment, string content)
    {
        // Check for the latest document with the given SecretId, Environment
        var existingSecrets = await _repository.GetSecretsByProjectNameAndEnvironmentAsync(projectName, environment);
        var existingSecret = existingSecrets
            .Where(s => s.SecretId == secretId)
            .OrderByDescending(s => s.Version)
            .FirstOrDefault();

        var secretVersion = (existingSecret?.Version ?? 0) + 1;

        var secret = new UserSecretDocument
        {
            ProjectName = projectName,
            SecretId = secretId,
            Environment = environment,
            Content = content,
            Version = secretVersion,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Insert into the MongoDB collection
        await _repository.AddSecretAsync(secret);
    }

    public async Task RemoveSecretByIdAsync(string id)
    {
        await _repository.DeleteSecretByIdAsync(id);
    }
    
    public async Task<List<UserSecretDocument>> FilterSecretsByEnvironmentAsync(string environment) =>
        await _repository.GetSecretsByEnvironmentAsync(environment);

    public async Task<List<UserSecretDocument>> FilterSecretsByProjectNameAsync(string projectName) =>
        await _repository.GetSecretsByProjectNameAsync(projectName);
}