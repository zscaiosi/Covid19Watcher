using System.Collections.Generic;
using System.Threading.Tasks;

namespace Covid19Watcher.Scraper.Services
{
    public interface IRestService
    {
        /// <summary>
        /// Makes POST Request
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<T> MakeHttpPostRequest<I, T>(string uri, I data, string medisType = "application/json");
    }
}