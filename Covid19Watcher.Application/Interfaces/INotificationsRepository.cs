using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Covid19Watcher.Application.Data.MongoDB.Documents;
using Covid19Watcher.Application.Contracts;

namespace Covid19Watcher.Application.Interfaces
{
    public interface INotificationsRepository
    {
        Task<List<NotificationDocument>> ListFilteredAsync(GetFiltersRequest filters);
        Task<Guid> CreateNotificationAsync(PostNotificationsRequest request);
        Task<NotificationDocument> FindByIdAsync(string id);
        /// <summary>
        /// Lists grouped by countries
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<List<GroupedNotifications>> ListGroupedByCountryAsync(GetFiltersRequest filters);
    }
}