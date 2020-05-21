using System;
using System.Threading.Tasks;
using Covid19Watcher.API.Public.Contracts;
using Covid19Watcher.API.Public.Interfaces;

namespace Covid19Watcher.API.Public.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly INotificationsRepository _repo;
        public NotificationsService(INotificationsRepository repo)
        {
            _repo = repo;
        }
        /// <summary>
        /// Responsible for business logic
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
    }
}