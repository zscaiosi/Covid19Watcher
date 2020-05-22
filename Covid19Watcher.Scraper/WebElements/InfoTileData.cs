using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Covid19Watcher.Scraper.WebElements
{
    public class InfoTileData
    {
        public InfoTileData(params IWebElement[] legends)
        {
            foreach (var _ in legends)
                _legends.Add(_);
        }

        private List<IWebElement> _legends {get;set;} = new List<IWebElement>();
        /// <summary>
        /// Get total active cases
        /// </summary>
        /// <returns></returns>
        public string GetActive() =>
            _legends.Where(
                l => l.FindElement(By.ClassName("description")).Text.ToLower().Contains("active")
            ).FirstOrDefault()?.FindElement(By.ClassName("total")).Text;
        /// <summary>
        /// Get total recovered cases
        /// </summary>
        /// <value></value>
        public string GetRecovered() =>
            _legends.Where(
                l => l.FindElement(By.ClassName("description")).Text.ToLower().Contains("recovered")
            ).FirstOrDefault()?.FindElement(By.ClassName("total")).Text;
        /// <summary>
        /// Get total deaths cases
        /// </summary>
        /// <value></value>
        public string GetDeaths() =>
            _legends.Where(
                l => l.FindElement(By.ClassName("description")).Text.ToLower().Contains("fatal")
            ).FirstOrDefault()?.FindElement(By.ClassName("total")).Text;
    }
}