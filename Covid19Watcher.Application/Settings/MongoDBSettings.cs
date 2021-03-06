using Covid19Watcher.Application.Interfaces;

namespace Covid19Watcher.Application.Settings
{
    public class MongoDBSettings : IMongoDBSettings
    {
        public string DataBase {get;set;}
        public string ConnString {get;set;}
        public string[] Collections {get;set;}
    }
}