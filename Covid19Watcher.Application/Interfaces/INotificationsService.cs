using System.Threading.Tasks;
using Covid19Watcher.Application.Contracts;
using Covid19Watcher.Application.Data.MongoDB.Documents;
using Covid19Watcher.Application.Services;

namespace Covid19Watcher.Application.Interfaces
{
    public interface INotificationsService
    {
        Task<ResultData> ListNotificationsAsync(GetFiltersRequest filters);
        Task<ResultData> CreateNotificationAsync(PostNotificationsRequest request);
        Task<ResultData> FindByIdAsync(string id);
    }
}