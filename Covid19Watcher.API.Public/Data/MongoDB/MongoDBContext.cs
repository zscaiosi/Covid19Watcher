using Covid19Watcher.API.Public.Interfaces;
using Covid19Watcher.API.Public.Data.MongoDB.Documents;
using MongoDB.Driver;
using System.Linq;
using Covid19Watcher.API.Public.Enums;

namespace Covid19Watcher.API.Public.Data.MongoDB
{
    /// <summary>
    /// Responsible for connecting to Database and exposing collections. Only accessible by DAOs
    /// </summary>
    public class MongoDBContext
    {
        private IMongoDBSettings _settings {get;set;}
        private readonly IMongoCollection<NotificationDocument> _notifications;
        public MongoDBContext(IMongoDBSettings settings)
        {
            _settings = settings;
            var client = new MongoClient(_settings.ConnString);
            var db = client.GetDatabase(_settings.DataBase);

            _notifications = db.GetCollection<NotificationDocument>(
                _settings.Collections.FirstOrDefault(name => name == ECollectionsNames.Notifications.ToString())
            );
        }
        // Notifications Getter
        public IMongoCollection<NotificationDocument> Notifications {
            get => _notifications;
        }
    }
}