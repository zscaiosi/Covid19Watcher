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
        public ChromeService(IConfiguration conf)
        {
            _conf = conf;
            // Sets chrome driver
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            _driver = new ChromeDriver(
                _conf.GetSection("SeleniumConfigurations").GetSection("GoogleDriver").Value,
                options
            );
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
            }

            Console.WriteLine(JsonConvert.SerializeObject(_postRequests.FirstOrDefault()));
            // Clear resources
            _driver.Close();
            _driver.Dispose();
        }
        private async Task LoadPageAsync()
        {
            await Task.Run(() => {
                // One minute
                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);
                // Navigate to page
                _driver.Navigate().GoToUrl(_conf.GetSection("SeleniumConfigurations").GetSection("URI").Value + _currentCountry);
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

            result.CaptureTime = DateTime.UtcNow;
            result.CountryName = _currentCountry;
            result.Infections = confirmedSituations[0].IgnoreAfter("+").SanitizeCommas();
            result.Recovered = confirmedSituations[1].IgnoreAfter("+").SanitizeCommas();
            result.Deaths = confirmedSituations[2].IgnoreAfter("+").SanitizeCommas();

            // Instantiates if first iteration
            if (_postRequests == null)
                _postRequests = new List<PostNotificationsRequest>();
            
            _postRequests.Add(result);
        }
    }
}