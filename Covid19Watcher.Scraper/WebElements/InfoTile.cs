using OpenQA.Selenium;

namespace Covid19Watcher.Scraper.WebElements
{
    public class InfoTile
    {
        public InfoTile(IWebElement ith, IWebElement c, IWebElement itd)
        {
            InfoTileheader = ith;
            Confirmed = c;
            InfoTileData = itd;
        }
        public IWebElement InfoTileheader {get;set;}
        public IWebElement Confirmed {get;set;}
        public IWebElement InfoTileData {get;set;}
    }
}