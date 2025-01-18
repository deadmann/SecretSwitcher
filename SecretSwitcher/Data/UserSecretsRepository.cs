using MongoDB.Driver;

namespace SecretSwitcher.Data;

public class UserSecretsRepository
{
    private readonly IMongoCollection<UserSecretDocument> _collection;

    public UserSecretsRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserSecretDocument>("UserSecrets");
    }

    public async Task AddSecretAsync(UserSecretDocument secret) => 
        await _collection.InsertOneAsync(secret);

    public async Task DeleteSecretByIdAsync(string id)
    {
        await _collection.DeleteOneAsync(s => s.Id == id);
    }

    
    public async Task<List<UserSecretDocument>> GetSecretsByEnvironmentAsync(string environment) =>
        await _collection.Find(s => s.Environment == environment).ToListAsync();

    public async Task<List<UserSecretDocument>> GetSecretsByProjectNameAsync(string projectName) =>
        await _collection.Find(s => s.ProjectName == projectName).ToListAsync();

    public async Task<List<UserSecretDocument>> GetSecretsByProjectNameAndEnvironmentAsync(string projectName, string environment) =>
        await _collection.Find(s => s.ProjectName == projectName && s.Environment == environment).ToListAsync();
    
    public async Task<UserSecretDocument> GetSecretByIdAsync(string id) =>
        await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();
}