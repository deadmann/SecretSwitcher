using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SecretSwitcher.Data;

public class UserSecretDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!; // MongoDB ObjectId as a string
    public string ProjectName { get; set; } = null!;
    public string? SecretId { get; set; } = null!;
    public string Environment { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}