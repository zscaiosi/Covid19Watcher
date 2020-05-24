using System;
using System.Linq;
using System.Threading.Tasks;
using Covid19Watcher.Application.Contracts;
using Covid19Watcher.Application.Errors;
using Covid19Watcher.Application.Interfaces;
using Covid19Watcher.Application.Enums;
using Covid19Watcher.Application.Data.MongoDB.Documents;

namespace Covid19Watcher.Application.Services
{
    /// <summary>
    /// Responsible for business logic
    /// </summary>
    public class NotificationsService : BaseService, INotificationsService
    {
        private readonly INotificationsRepository _repo;
        public NotificationsService(INotificationsRepository repo) : base()
        {
            _repo = repo;
        }
        /// <summary>
        /// Lists all notifications
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<ResultData> ListNotificationsAsync(GetFiltersRequest filters)
        {
            if (filters.Page < 0 || filters.Limit < 0 || !Enum.IsDefined(typeof(EOrdenation), filters.OrderBy))
                return ErrorData(ServiceErrors.Get_ListNotificationsAsync_400_Filters);
            
            var result = await _repo.ListFilteredAsync(filters);

            if (result.Count < 1)
                return ErrorData(ServiceErrors.Get_ListNotificationsAsync_404_Notification);

            return SuccessData(
                new GetNotificationsResponse(filters.Page, result.Count)
                {
                    Content = result
                }
            );
        }
        /// <summary>
        /// Creates a new notification
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData> CreateNotificationAsync(PostNotificationsRequest request)
        {
            if (request.CaptureTime == null || string.IsNullOrEmpty(request.CountryName))
                return ErrorData(ServiceErrors.Post_CreateNotificationAsync_400_Payload);
            
            if (await ShouldCreateNew(request))
                return SuccessData<Guid>(await _repo.CreateNotificationAsync(request));
            else
                return SuccessData<string>(string.Empty);
        }
        /// <summary>
        /// Finds notification by GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultData> FindByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Split('-').Length != 5)
                return ErrorData(ServiceErrors.GetFind_FindByIdAsync_400_Id);
            
            return SuccessData<NotificationDocument>(await _repo.FindByIdAsync(id));
        }
        /// <summary>
        /// Checks if differs
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        private async Task<bool> ShouldCreateNew(PostNotificationsRequest doc)
        {
            var cases = await _repo.FindCountryCases(doc.CountryName);

            return !cases.Any(c => c.Deaths == doc.Deaths && c.Recovered == doc.Recovered && c.Infections == doc.Infections);
        }
    }
}