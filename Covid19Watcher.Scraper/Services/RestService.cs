using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Watcher.Scraper.Services.Constants;
using Newtonsoft.Json;
using System.Text;

namespace Covid19Watcher.Scraper.Services
{
    /// <summary>
    /// Responsible for making Http requests
    /// </summary>
    public class RestService : IRestService
    {
        private readonly HttpClient _client;
        public RestService(IHttpClientFactory factory, string baseAddress)
        {
            _client = factory.CreateClient();
            _client.BaseAddress = new Uri(baseAddress);
        }
        /// <summary>
        /// Makes POST Request
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> MakeHttpPostRequest<I, T>(string uri, I data = default(I), Dictionary<string, string> headers = null)
        {
            try
            {
                var message = await _client.PostAsync(uri, new StringContent(
                        JsonConvert.SerializeObject(data),
                        Encoding.UTF8,
                        "application/json"
                    ));
                
                var response = await message.Content.ReadAsStringAsync();
                
                if (string.IsNullOrEmpty(response))
                    throw new ArgumentNullException(uri);
                
                return JsonConvert.DeserializeObject<T>(response);
            }
            catch(Exception e)
            {
                Console.WriteLine($"{e.Source} {e.Message}");
                return default(T);
            }
        }
    }
}