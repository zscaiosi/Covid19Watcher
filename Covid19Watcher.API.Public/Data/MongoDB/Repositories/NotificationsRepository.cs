using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Watcher.API.Public.Contracts;
using Covid19Watcher.API.Public.Data.MongoDB.Documents;
using Covid19Watcher.API.Public.Interfaces;
using System.Linq;

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
                documents = await _notificationsDAO.ListDocuments(filters.onlyActives);
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
    }
}