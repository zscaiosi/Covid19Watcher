using OpenQA.Selenium;
namespace Covid19Watcher.Scraper.WebElements
{
    public class CountryTab
    {
        public CountryTab(IWebElement tpw, IWebElement it, IWebElement ca)
        {
            TimelinePillWrapper = tpw;
            InfoTile = it;
            CombinedArea = ca;
        }
        public IWebElement TimelinePillWrapper {get;set;}
        public IWebElement InfoTile {get;set;}
        public IWebElement CombinedArea {get;set;}
    }
}