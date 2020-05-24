using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Covid19Watcher.Application.Data.MongoDB.Documents;
using Covid19Watcher.Application.Contracts;

namespace Covid19Watcher.Application.Interfaces
{
    public interface INotificationsRepository
    {
        /// <summary>
        /// Lists all notifications with filters
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        Task<List<NotificationDocument>> ListFilteredAsync(GetFiltersRequest filters);
        /// <summary>
        /// Creates one notification
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<Guid> CreateNotificationAsync(PostNotificationsRequest request);
        /// <summary>
        /// Finds notification by GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<NotificationDocument> FindByIdAsync(string id);
        /// <summary>
        /// Finds only cases view
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        Task<List<CountryCasesView>> FindCountryCases(string countryName);
    }
}