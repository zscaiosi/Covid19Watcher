using Covid19Watcher.API.Public.Interfaces;

namespace Covid19Watcher.API.Public.Settings
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string DataBase {get;set;}
        public string ConnString {get;set;}
        public string[] Collections {get;set;}
    }
}