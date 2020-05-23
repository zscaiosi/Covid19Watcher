using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Covid19Watcher.Application.Data.MongoDB.Documents
{
    public class CountryDocument
    {
        [BsonId]
        public string Id {get;set;}
        [BsonElement("increaseRatio")]
        public int IncreaseRation {get;set;}
    }
}