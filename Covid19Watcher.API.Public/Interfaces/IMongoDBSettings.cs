namespace Covid19Watcher.API.Public.Interfaces
{
    public interface IMongoDBSettings
    {
        string DataBase {get;set;}
        string ConnString {get;set;}
        string[] Collections {get;set;}
    }
}