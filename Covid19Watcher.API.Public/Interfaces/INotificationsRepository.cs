using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Covid19Watcher.API.Public.Data.MongoDB.Documents;
using Covid19Watcher.API.Public.Contracts;

namespace Covid19Watcher.API.Public.Interfaces
{
    public interface INotificationsRepository
    {
        Task<List<NotificationDocument>> ListFilteredAsync(GetFiltersRequest filters);
    }
}