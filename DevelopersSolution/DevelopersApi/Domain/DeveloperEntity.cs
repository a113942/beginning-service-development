using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevelopersApi.Domain;

public class DeveloperEntity // an object that has an ID and you use the id to determine equivalence.
{
    [BsonElement("_id")]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public bool IsOnCallDeveloper { get; set; } = false;

}