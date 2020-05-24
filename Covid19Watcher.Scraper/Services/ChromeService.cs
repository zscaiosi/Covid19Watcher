using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using Covid19Watcher.Application.Contracts;
using OpenQA.Selenium;
using System.Drawing;
using System.Linq;
using Covid19Watcher.Application.Helpers;
using Covid19Watcher.Application.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using Covid19Watcher.Application.Services;
using System.Net.Http;
using Covid19Watcher.Scraper.WebElements;
using OpenQA.Selenium.Support.UI;

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
        private WebDriverWait _waiter;
        public ChromeService(IConfiguration conf, IHttpClientFactory factory)
        {
            _conf = conf;
            // Sets chrome driver
            var options = new ChromeOptions();
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            options.AddArgument("--headless");
            _driver = new ChromeDriver(
                _conf.GetSection("SeleniumConfigurations").GetSection("GoogleDriver").Value,
                options
            );
            // Navigates to page
            _driver.Navigate().GoToUrl(_conf.GetSection("SeleniumConfigurations").GetSection("URI").Value);

            _factory = factory;
            _httpTasks = new List<Task<ResultData>>();
        }
        /// <summary>
        /// Runs browser async
        /// </summary>
        /// <returns></returns>
        public async Task RunAsync(string[] args = null)
        {
            // Dynamically searches for all intended countries, one at a time.
            string[] countries = null;
            // Did receive countries in arguments
            if (args != null && args.Length > 0)
                countries = args;
            else
                countries = Enum.GetNames(typeof(ECountries));

            Console.WriteLine($"Will notify for: {JsonConvert.SerializeObject(countries)}");

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
            Close();
            // Waits for all HTTP requests
            var resultDatas = await Task.WhenAll(_httpTasks);

            foreach (var rd in resultDatas)
            {
                Console.WriteLine(JsonConvert.SerializeObject(rd));
            }
        }
        /// <summary>
        /// Closes driver
        /// </summary>
        public void Close()
        {
            _driver.Quit();
            _driver.Dispose();
        }
        private async Task LoadPageAsync()
        {
            await Task.Run(() => {
                // Resize
                _driver.Manage().Window.Size = new Size(1300, 950);
                
                Console.WriteLine($"{_driver.Manage().Window.Size.Width} x {_driver.Manage().Window.Size.Height}");
                // Waits
                _waiter = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                // One minute
                _driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60);

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
                try
                {
                    // We must ensure it is globally set before clicking country (https://www.bing.com/covid/*)
                    if (_driver.Url.Count() > _conf.GetSection("SeleniumConfigurations").GetSection("URI").Value.Length)
                        ClickCloseSelectedCountry();
                    
                    // Retrieves stats from clicked country
                    var infoTileElement = ClickCountryAndGetStats($"div#{_currentCountry.ToLower().Trim()}.area");

                    var barElement = infoTileElement
                        .FindElement(By.ClassName("bar"));
                    var confirmedElement = infoTileElement
                        .FindElement(By.ClassName("confirmed"));
                    var infoTileDataElement = infoTileElement
                        .FindElement(By.ClassName("infoTileData"));

                    var infoTile = new InfoTile(barElement, confirmedElement, infoTileDataElement);

                    var infoTileData = new InfoTileData(infoTile.InfoTileData.FindElements(By.ClassName("legend")).ToArray());

                    ExtractInfos(
                        new string[]
                        {
                            infoTileData.GetActive(),
                            infoTileData.GetRecovered(),
                            infoTileData.GetDeaths()
                        }
                    );
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Something went wrong here: {_driver.Url}... \n But don't worry, we can keep trying at the next Country.");
                }
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
            var active = confirmedSituations[0] ?? "0";
            var recovered = confirmedSituations[1] ?? "0";
            var deaths = confirmedSituations[2] ?? "0";
            
            Console.WriteLine($"Posting infos for {_currentCountry}");
            Console.WriteLine($"active={active.Replace("\n", "")}; recovered={recovered.Replace("\n", "")}; deaths={deaths.Replace("\n", "")}");

            active = active.Contains("+") ? active.IgnoreAfter("+").SanitizeCommas() : active.SanitizeCommas();
            recovered = recovered.Contains("+") ? recovered.IgnoreAfter("+").SanitizeCommas() : recovered.SanitizeCommas();
            deaths = deaths.Contains("+") ? deaths.IgnoreAfter("+").SanitizeCommas() : deaths.SanitizeCommas();

            result.CaptureTime = DateTime.UtcNow;
            result.CountryName = _currentCountry;

            if (int.TryParse(active, out var inf))
                result.Infections = inf;
            if (int.TryParse(recovered, out var rec))
                result.Recovered = rec;
            if (int.TryParse(deaths, out var dea))
                result.Deaths = dea;
            
            result.IsActive = true;

            // Instantiates if first iteration
            if (_postRequests == null)
                _postRequests = new List<PostNotificationsRequest>();
            
            _postRequests.Add(result);
        }
        /// <summary>
        /// Clicks to close default country. If can't close region, then goes directly to next one.
        /// </summary>
        private void ClickCloseSelectedCountry()
        {
            Console.WriteLine($"We are at: {_driver.Url}");
            IWebElement countryElement;
            IWebElement closeSpan;

            try
            {
                // Just below Global area
                countryElement = _waiter.Until(d => d.FindElement(By.CssSelector("div.combinedArea div.selectedAreas div.areaDiv:nth-child(3)")));
                // Get span with close svg
                closeSpan = countryElement.FindElement(By.CssSelector("div.area.selectedArea div.areaTotal div.secondaryInfo span"));

                // Now clicks it to dismiss
                closeSpan.Click();

                Console.WriteLine($"Closing region and going to Global at: {_driver.Url}");
            }
            catch (OpenQA.Selenium.WebDriverTimeoutException e)
            {
                Console.WriteLine($"WebDriverTimeoutException {e.Message} at:\n {_driver.Url}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception {e.Message} at:\n {_driver.Url}");
            }
        }
        /// <summary>
        /// Clicks a country to get infos from
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        private IWebElement ClickCountryAndGetStats(string selector)
        {
            var localDiv = _waiter.Until(d => d.FindElement(By.CssSelector(selector)));
            // Selects the country to find stats about
            localDiv.Click();

            Console.WriteLine($"Getting stats at: {_driver.Url}");

            var overviewContentElement = _driver
                .FindElementById("main")
                .FindElement(By.ClassName("desktop"))
                .FindElement(By.ClassName("wholePage"))
                .FindElement(By.ClassName("content"))
                .FindElement(By.ClassName("verticalWrapper"))
                .FindElement(By.ClassName("verticalContent"))
                .FindElement(By.ClassName("overview"))
                .FindElement(By.ClassName("overviewContent"));

            // class="infoTile"
            return overviewContentElement
                .FindElement(By.ClassName("infoTile"));
        }
    }
}