using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Covid19Watcher.Application.Contracts;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

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
        public async Task<T> MakeHttpPostRequest<I, T>(string uri, I data = default(I), string mediaType = "application/json")
        {
            try
            {
                // First authorizes
                var jwt = await PostAuthAsync(new PostAuth { Login = "user", Password = "12345" });
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.AccessToken);
                var message = await _client.PostAsync(uri, new StringContent(
                        JsonConvert.SerializeObject(data),
                        Encoding.UTF8,
                        mediaType
                    ));
                
                var response = await message.Content.ReadAsStringAsync();

                Console.WriteLine($"HTTP Response: {response}");
                
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
        /// <summary>
        /// Gets JWT
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<PostAuthResponse> PostAuthAsync(PostAuth request)
        {
            var message = await _client.PostAsync("auth", new StringContent(
                    JsonConvert.SerializeObject(request),
                    Encoding.UTF8,
                    "application/json"
                ));

            if (!message.IsSuccessStatusCode)
                return new PostAuthResponse { AccessToken = string.Empty };
            
            var body = await message.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<PostAuthResponse>(body);
        }
    }
}