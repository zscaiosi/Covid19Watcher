using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Watcher.Application.Data.MongoDB.Documents;
using Covid19Watcher.Application.Enums;
using Covid19Watcher.Application.Interfaces;
using MongoDB.Driver;

namespace Covid19Watcher.Application.Data.MongoDB.DAOs
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
        public async Task<List<NotificationDocument>> ListDocuments(bool isActive, EOrdenation ordenation)
        {
            FilterDefinition<NotificationDocument> filter = Builders<NotificationDocument>.Filter.And(
                Builders<NotificationDocument>.Filter.Eq(nameof(NotificationDocument.Active), isActive)
            );

            var oDef = this.OrderBy(ordenation);

            return (await _ctx.Notifications.FindAsync(filter, new FindOptions<NotificationDocument, NotificationDocument>()
            {
                Sort = oDef
            })).ToList();
        }
        /// <summary>
        /// Finds the Country's active notification
        /// </summary>
        /// <param name="countryName"></param>
        /// <typeparam name="NotificationDocument"></typeparam>
        /// <returns></returns>
        public async Task<List<NotificationDocument>> FindByCountry(string countryName, bool isActive) =>
            (await _ctx.Notifications.FindAsync<NotificationDocument>(n => n.CountryName == countryName && n.Active == isActive)).ToList();
        /// <summary>
        /// Inserts one document into collection
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public async Task CreateNotificationAsync(NotificationDocument document) =>
            await _ctx.Notifications.InsertOneAsync(document);
        /// <summary>
        /// Finds a specific notification
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NotificationDocument> FindByIdAsync(Guid id) =>
            (await _ctx.Notifications.FindAsync(n => n.Id == id)).FirstOrDefault();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ordenation"></param>
        /// <returns></returns>
        private SortDefinition<NotificationDocument> OrderBy(EOrdenation ordenation)
        {
            switch (ordenation)
            {
                case EOrdenation.Infections:
                    return Builders<NotificationDocument>.Sort.Descending(nameof(NotificationDocument.Infections));
                case EOrdenation.Deaths:
                    return Builders<NotificationDocument>.Sort.Descending(nameof(NotificationDocument.Deaths));
                case EOrdenation.Recovered:
                    return Builders<NotificationDocument>.Sort.Descending(nameof(NotificationDocument.Recovered));
                case EOrdenation.Total:
                    return Builders<NotificationDocument>.Sort.Descending(nameof(NotificationDocument.Total));
                default:
                    return Builders<NotificationDocument>.Sort.Descending(nameof(NotificationDocument.Infections));
            }
        }
    }
}