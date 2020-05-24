using OpenQA.Selenium;

namespace Covid19Watcher.Scraper.WebElements
{
    public class InfoTile
    {
        public InfoTile(IWebElement b, IWebElement c, IWebElement itd)
        {
            Bar = b;
            Confirmed = c;
            InfoTileData = itd;
        }
        public IWebElement Bar {get;set;}
        public IWebElement Confirmed {get;set;}
        public IWebElement InfoTileData {get;set;}
    }
}