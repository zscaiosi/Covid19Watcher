using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Watcher.API.Public.Contracts;
using Covid19Watcher.API.Public.Data.MongoDB.Documents;
using Covid19Watcher.API.Public.Interfaces;
using System.Linq;
using System;

namespace Covid19Watcher.API.Public.Data.MongoDB.Repositories
{
    /// <summary>
    /// Responsible for retrieving data and applying filters and sanitizations
    /// </summary>
    public class NotificationsRepository : INotificationsRepository
    {
        private readonly INotificationsDAO _notificationsDAO;
        public NotificationsRepository(INotificationsDAO notificationsDAO)
        {
            _notificationsDAO = notificationsDAO;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters">GetFiltersRequest</param>
        /// <returns></returns>
        public async Task<List<NotificationDocument>> ListFilteredAsync(GetFiltersRequest filters)
        {
            List<NotificationDocument> documents = new List<NotificationDocument>();
            // Retrieves and manipulates by filters
            if (!string.IsNullOrEmpty(filters.Country))
            {
                documents.Add(
                    await _notificationsDAO.FindByCountry(filters.Country, filters.onlyActives)
                );
            }
            else
            {
                documents = await _notificationsDAO.ListDocuments(filters.onlyActives, filters.OrderBy);
            }
            if (filters.Page > 0)
            {
                documents = documents.Skip(filters.Page * (filters.Limit > 0 ? filters.Limit : 1)).ToList();
            }
            if (filters.Limit > 0)
            {
                documents = documents.Take(filters.Limit).ToList();
            }

            return documents;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Guid> CreateNotificationAsync(PostNotificationsRequest request)
        {
            var id = Guid.NewGuid();
            await _notificationsDAO.CreateNotificationAsync(new NotificationDocument
            {
                Id = id,
                CountryName = request.CountryName,
                Active = request.IsActive,
                CapturedAt = request.CaptureTime,
                Infections = request.Infections,
                Deaths = request.Deaths,
                Recovered = request.Recovered
            });

            return id;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NotificationDocument> FindByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var _id))
                throw new ArgumentException();
            
            return await _notificationsDAO.FindByIdAsync(_id);
        }
    }
}