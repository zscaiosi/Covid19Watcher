using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Covid19Watcher.API.Public.Data.MongoDB.Documents
{
    public class NotificationDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id {get;set;}
        [BsonElement("countryName")]
        public string CountryName {get;set;}
        [BsonElement("active")]
        public bool Active {get;set;}
    }
}