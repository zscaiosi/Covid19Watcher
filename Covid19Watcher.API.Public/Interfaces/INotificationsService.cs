using System.Threading.Tasks;
using Covid19Watcher.API.Public.Contracts;
using Covid19Watcher.API.Public.Data.MongoDB.Documents;
using Covid19Watcher.API.Public.Services;

namespace Covid19Watcher.API.Public.Interfaces
{
    public interface INotificationsService
    {
        Task<GetNotificationsResponse> ListNotificationsAsync(GetFiltersRequest filters);
        Task<ResultData> CreateNotificationAsync(PostNotificationsRequest request);
        Task<ResultData> FindByIdAsync(string id);
    }
}