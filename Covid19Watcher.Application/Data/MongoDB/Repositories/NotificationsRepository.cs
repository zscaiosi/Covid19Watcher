using System.Collections.Generic;
using System.Threading.Tasks;
using Covid19Watcher.Application.Contracts;
using Covid19Watcher.Application.Data.MongoDB.Documents;
using Covid19Watcher.Application.Interfaces;
using System.Linq;
using System;

namespace Covid19Watcher.Application.Data.MongoDB.Repositories
{
    /// <summary>
    /// Responsible for retrieving data and applying filters and sanitizations
    /// </summary>
    public class NotificationsRepository : INotificationsRepository
    {
        private readonly INotificationsDAO _notificationsDAO;
        public NotificationsRepository(INotificationsDAO notificationsDAO)
        {
            _notificationsDAO = notificationsDAO;
        }
        /// <summary>
        /// Lists all notifications by filter
        /// </summary>
        /// <param name="filters">GetFiltersRequest</param>
        /// <returns></returns>
        public async Task<List<NotificationDocument>> ListFilteredAsync(GetFiltersRequest filters)
        {
            List<NotificationDocument> documents = new List<NotificationDocument>();
            // Retrieves and manipulates by filters
            if (!string.IsNullOrEmpty(filters.Country))
            {
                documents = await _notificationsDAO.FindByCountry(filters.Country, filters.onlyActives);
            }
            else
            {
                documents = await _notificationsDAO.ListDocuments(filters.onlyActives, filters.OrderBy);
            }
            if (filters.Page > 0)
            {
                documents = documents.Skip(filters.Page * filters.Limit).ToList();
            }
            if (filters.Limit > 0)
            {
                documents = documents.Take(filters.Limit).ToList();
            }

            return documents;
        }
        /// <summary>
        /// Lists grouped by countries
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public async Task<List<GroupedNotifications>> ListGroupedByCountryAsync(GetFiltersRequest filters)
        {
            var listed = await ListFilteredAsync(filters);
            // Now groups by Country
            Func<NotificationDocument, string> keySelector = (element) => nameof(element.CountryName);
            Func<NotificationDocument, NotificationDocument> elementSelector = (element) => element;
            // Groups by country calculating the ratios
            var grouped  = listed.GroupBy(
                keySelector,
                elementSelector,
                (key, elements) => {
                    var prevH = elements.LastOrDefault().CapturedAt;
                    var now = DateTime.UtcNow;
                    var firstTotal = elements.LastOrDefault().Total;
                    var firstInfected = elements.LastOrDefault().Infections;
                    var firstRecovered = elements.LastOrDefault().Recovered;
                    var firstDeaths = elements.LastOrDefault().Deaths;

                    return new GroupedNotifications{
                        Country = key,
                        InfectedRation = CalcRatio(prevH.Hour, now.Hour, firstInfected, elements.FirstOrDefault().Infections),
                        RecoveredRation = CalcRatio(prevH.Hour, now.Hour, firstRecovered, elements.FirstOrDefault().Recovered),
                        DeathsRation = CalcRatio(prevH.Hour, now.Hour, firstDeaths, elements.FirstOrDefault().Deaths),
                        LastNotification = elements.FirstOrDefault()
                    };
                }
            );

            return grouped.ToList();
        }
        /// <summary>
        /// Creates a new notification
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Guid> CreateNotificationAsync(PostNotificationsRequest request)
        {
            var id = Guid.NewGuid();
            await _notificationsDAO.CreateNotificationAsync(new NotificationDocument
            {
                Id = id,
                CountryName = request.CountryName,
                Active = request.IsActive,
                CapturedAt = request.CaptureTime,
                Infections = request.Infections,
                Deaths = request.Deaths,
                Recovered = request.Recovered,
                Total = request.Infections + request.Deaths + request.Recovered
            });

            return id;
        }
        /// <summary>
        /// Finds notification by GUID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<NotificationDocument> FindByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out var _id))
                throw new ArgumentException();
            
            return await _notificationsDAO.FindByIdAsync(_id);
        }
        /// <summary>
        /// Calculates a linear coefficient
        /// </summary>
        /// <param name="prevHour"></param>
        /// <param name="currentHour"></param>
        /// <param name="prevCases"></param>
        /// <param name="cases"></param>
        /// <returns></returns>
        private decimal CalcRatio(int prevHour, int currentHour, int prevCases, int cases) => (cases - prevCases) / (currentHour - prevHour);
        /// <summary>
        /// Finds only cases view
        /// </summary>
        /// <param name="countryName"></param>
        /// <returns></returns>
        public async Task<List<CountryCasesView>> FindCountryCases(string countryName)
        {
            var result = new List<CountryCasesView>();
            var notifications = await _notificationsDAO.FindByCountry(countryName, true);
            
            foreach (var n in notifications)
            {
                result.Add(new CountryCasesView(n));
            }

            return result;
        }
    }
}