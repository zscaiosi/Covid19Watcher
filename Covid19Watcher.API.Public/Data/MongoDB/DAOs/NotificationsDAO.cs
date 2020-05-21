using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Watcher.API.Public.Data.MongoDB.Documents;
using Covid19Watcher.API.Public.Interfaces;
using MongoDB.Driver;

namespace Covid19Watcher.API.Public.Data.MongoDB.DAOs
{
    /// <summary>
    /// Responsible for Data Access - Data Access Objetc
    /// </summary>
    public class NotificationsDAO : INotificationsDAO
    {
        private readonly MongoDBContext _ctx;
        private readonly IMongoDBSettings _settings;
        public NotificationsDAO(IMongoDBSettings settings)
        {
            _settings = settings;
            _ctx = new MongoDBContext(_settings);
        }
        /// <summary>
        /// Lists all active notifications
        /// </summary>
        /// <typeparam name="NotificationDocument"></typeparam>
        /// <returns></returns>
        public async Task<List<NotificationDocument>> ListDocuments(bool isActive) =>
            (await _ctx.Notifications.FindAsync<NotificationDocument>(d => d.Active == isActive)).ToList();
        /// <summary>
        /// Finds the Country's active notification
        /// </summary>
        /// <param name="countryName"></param>
        /// <typeparam name="NotificationDocument"></typeparam>
        /// <returns></returns>
        public async Task<NotificationDocument> FindByCountry(string countryName, bool isActive) =>
            (await _ctx.Notifications.FindAsync<NotificationDocument>(n => n.CountryName == countryName && n.Active == isActive)).FirstOrDefault();
    }
}