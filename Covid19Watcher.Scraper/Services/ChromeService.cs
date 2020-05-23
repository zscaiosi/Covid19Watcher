using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using Covid19Watcher.Application.Contracts;
using OpenQA.Selenium;
using Covid19Watcher.Scraper.WebElements;
using System.Linq;
using Covid19Watcher.Application.Helpers;
using Covid19Watcher.Application.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using Covid19Watcher.Application.Services;
using System.Net.Http;

namespace Covid19Watcher.Scraper.Services
{
    /// <summary>
    /// Reponsible for acting as a Chrome browser instance
    /// </summary>
    public class ChromeService
    {
        private readonly IConfiguration _conf;
        protected ChromeDriver _driver {get;set;}
        protected List<PostNotificationsRequest> _postRequests {get;set;}
        protected string _currentCountry {get;set;}
        private IHttpClientFactory _factory;
        private List<Task<ResultData>> _httpTasks;
        public ChromeService(IConfiguration conf, IHttpClientFactory factory)
        {
            _conf = conf;
            // Sets chrome driver
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            _driver = new ChromeDriver(
                _conf.GetSection("SeleniumConfigurations").GetSection("GoogleDriver").Value,
                options
            );

            _factory = factory;
            _httpTasks = new List<Task<ResultData>>();
        }
        /// <summary>
        /// Runs browser async
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync()
        {
            // Dynamically searches for all intended countries.
            // It is way more efficient than navigating throught the page and sinulating clicks and types
            var countries = Enum.GetNames(typeof(ECountries));

            foreach (var c in countries)
            {
                _currentCountry = c;

                await LoadPageAsync();

                await LoadPanelAsync();

                // Now prepares client and makes POST HTTP Request
                var client = new RestService(_factory, _conf.GetSection("API_URI").Value);

                var payload = _postRequests.FirstOrDefault(pr => pr.CountryName == _currentCountry);

                _httpTasks.Add(
                    client.MakeHttpPostRequest<PostNotificationsRequest, ResultData>("notifications", payload)
                );

                Console.WriteLine(JsonConvert.SerializeObject(payload));
            }

            // Clear resources
            _driver.Close();
            _driver.Dispose();
            // Waits for all HTTP requests
            var resultDatas = await Task.WhenAll(_httpTasks);

            foreach (var rd in resultDatas)
            {
                Console.WriteLine(JsonConvert.SerializeObject(rd));
            }
        }
        private async Task LoadPageAsync()
        {
            await Task.Run(() => {
                // One minute
                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
                // Navigate to page
                _driver.Navigate().GoToUrl(_conf.GetSection("SeleniumConfigurations").GetSection("URI").Value + _currentCountry);
                Console.WriteLine("Page Loaded...");
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task LoadPanelAsync()
        {
            await Task.Run(() => {
                var countryTabElement = _driver
                    .FindElementById("main")
                    .FindElement(By.ClassName("desktop"))
                    .FindElement(By.ClassName("wholePage"))
                    .FindElement(By.ClassName("content"))
                    .FindElement(By.ClassName("countryPanel"))
                    .FindElement(By.CssSelector("div.country.tab"));
                
                // class="country lab"
                var infoTileElement = countryTabElement
                    .FindElement(By.ClassName("infoTile"));
                
                var countryTab = new CountryTab(null, infoTileElement, null);
                // class="infoTile"
                var infoTileHeaderElement = infoTileElement
                    .FindElement(By.ClassName("infoTileHeader"));
                var confirmedElement = infoTileElement
                    .FindElement(By.ClassName("confirmed"));
                var infoTileDataElement = infoTileElement
                    .FindElement(By.ClassName("infoTileData"));

                var infoTile = new InfoTile(infoTileHeaderElement, confirmedElement, infoTileDataElement);

                var infoTileData = new InfoTileData(infoTile.InfoTileData.FindElements(By.ClassName("legend")).ToArray());

                ExtractInfos(
                    new string[]
                    {
                        infoTileData.GetActive(),
                        infoTileData.GetRecovered(),
                        infoTileData.GetDeaths()
                    }
                );
            });
        }
        /// <summary>
        /// Extracts infos into POST Requests payloads
        /// </summary>
        /// <param name="confirmedSituations"></param>
        /// <returns></returns>
        private void ExtractInfos(params string[] confirmedSituations)
        {
            var result = new PostNotificationsRequest();
            var active = confirmedSituations[0];
            var recovered = confirmedSituations[1];
            var deaths = confirmedSituations[2];
            
            Console.WriteLine($"Posting infos for {_currentCountry}");
            Console.WriteLine($"{active} - {recovered} - {deaths}");

            result.CaptureTime = DateTime.UtcNow;
            result.CountryName = _currentCountry;
            result.Infections = active.Contains("+") ? active.IgnoreAfter("+").SanitizeCommas() : active.SanitizeCommas();
            result.Recovered = recovered.Contains("+") ? recovered.IgnoreAfter("+").SanitizeCommas() : recovered.SanitizeCommas();
            result.Deaths = deaths.Contains("+") ? deaths.IgnoreAfter("+").SanitizeCommas() : deaths.SanitizeCommas();
            result.IsActive = true;

            // Instantiates if first iteration
            if (_postRequests == null)
                _postRequests = new List<PostNotificationsRequest>();
            
            _postRequests.Add(result);
        }
    }
}