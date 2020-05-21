using System.Threading.Tasks;
using Covid19Watcher.API.Public.Contracts;

namespace Covid19Watcher.API.Public.Interfaces
{
    public interface INotificationsService
    {
        Task<GetNotificationsResponse> ListNotificationsAsync(GetFiltersRequest filters);
    }
}