using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Watcher.Application.Data.MongoDB.Documents;
using Covid19Watcher.Application.Enums;

namespace Covid19Watcher.Application.Interfaces
{
    public interface INotificationsDAO
    {
        Task<List<NotificationDocument>> ListDocuments(bool isActive, EOrdenation ordenation);
        Task<NotificationDocument> FindByCountry(string countryName, bool isActive);
        Task CreateNotificationAsync(NotificationDocument document);
        Task<NotificationDocument> FindByIdAsync(Guid id);
    }
}