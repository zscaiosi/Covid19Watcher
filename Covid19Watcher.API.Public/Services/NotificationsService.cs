using System;
using System.Threading.Tasks;
using Covid19Watcher.API.Public.Contracts;
using Covid19Watcher.API.Public.Errors;
using Covid19Watcher.API.Public.Interfaces;
using Covid19Watcher.API.Public.Enums;
using Covid19Watcher.API.Public.Data.MongoDB.Documents;

namespace Covid19Watcher.API.Public.Services
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
        public async Task<GetNotificationsResponse> ListNotificationsAsync(GetFiltersRequest filters)
        {
            var result = await _repo.ListFilteredAsync(filters);

            return new GetNotificationsResponse(filters.Page, filters.Limit)
            {
                Content = result
            };
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