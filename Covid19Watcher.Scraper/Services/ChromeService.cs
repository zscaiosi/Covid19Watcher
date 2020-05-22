using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using Covid19Watcher.Application.Contracts;
using OpenQA.Selenium;
using Covid19Watcher.Scraper.WebElements;
using System.Linq;
using Covid19Watcher.Application.Helpers;
using Newtonsoft.Json;

namespace Covid19Watcher.Scraper.Services
{
    /// <summary>
    /// Reponsible for acting as a Chrome browser instance
    /// </summary>
    public class ChromeService
    {
        private readonly IConfiguration _conf;
        protected ChromeDriver _driver {get;set;}
        protected PostNotificationsRequest _postRequest {get;set;}
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
            await LoadPageAsync();

            await LoadPanelAsync();

            Console.WriteLine(JsonConvert.SerializeObject(_postRequest));
        }
        private async Task LoadPageAsync()
        {
            await Task.Run(() => {
                // Two minutes
                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(120);
                // Navigate to page
                _driver.Navigate().GoToUrl(_conf.GetSection("SeleniumConfigurations").GetSection("URI").Value);
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

                _postRequest = ExtractInfos(
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
        /// 
        /// </summary>
        /// <param name="confirmedSituations"></param>
        /// <returns></returns>
        private PostNotificationsRequest ExtractInfos(params string[] confirmedSituations)
        {
            var result = new PostNotificationsRequest();

            result.Infections = confirmedSituations[0].IgnoreAfter("+").SanitizeCommas();
            result.Recovered = confirmedSituations[1].IgnoreAfter("+").SanitizeCommas();
            result.Deaths = confirmedSituations[2].IgnoreAfter("+").SanitizeCommas();

            return result;
        }
    }
}