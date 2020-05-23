using System;
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
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<ResultData> ListNotificationsAsync(GetFiltersRequest filters)
        {
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
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultData> CreateNotificationAsync(PostNotificationsRequest request)
        {
            if (request.CaptureTime == null || string.IsNullOrEmpty(request.CountryName) || !Enum.IsDefined(typeof(ECountries), request.CountryName))
                return ErrorData(ServiceErrors.Post_CreateNotificationAsync_400_Payload);
            
            return SuccessData<Guid>(await _repo.CreateNotificationAsync(request));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ResultData> FindByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Split('-').Length != 5)
                return ErrorData(ServiceErrors.GetFind_FindByIdAsync_400_Id);
            
            return SuccessData<NotificationDocument>(await _repo.FindByIdAsync(id));
        }
    }
}