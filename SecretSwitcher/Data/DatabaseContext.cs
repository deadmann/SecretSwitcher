using MongoDB.Driver;

namespace SecretSwitcher.Data;

public class DatabaseContext
{
    public IMongoDatabase Database { get; }

    public DatabaseContext(string connectionString, string? databaseName)
    {
        var client = new MongoClient(connectionString);
        Database = client.GetDatabase(databaseName);
    }
}