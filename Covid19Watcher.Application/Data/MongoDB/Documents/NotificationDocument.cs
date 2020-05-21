using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Covid19Watcher.Application.Data.MongoDB.Documents
{
    public class NotificationDocument
    {
        [BsonId]
        public Guid Id {get;set;}
        [BsonElement("countryName")]
        public string CountryName {get;set;}
        [BsonElement("active")]
        public bool Active {get;set;}
        [BsonElement("capturedAt")]
        public DateTime CapturedAt {get;set;}
        [BsonElement("infections")]
        public int Infections {get;set;} = 0;
        [BsonElement("deaths")]
        public int Deaths {get;set;} = 0;
        [BsonElement("recovered")]
        public int Recovered {get;set;} = 0;
    }
}