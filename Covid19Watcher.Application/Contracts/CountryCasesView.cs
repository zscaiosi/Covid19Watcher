using Covid19Watcher.Application.Data.MongoDB.Documents;
namespace Covid19Watcher.Application.Contracts
{
    public class CountryCasesView
    {
        public CountryCasesView(NotificationDocument document)
        {
            Country = document.CountryName;
            Infections = document.Infections;
            Recovered = document.Recovered;
            Deaths = document.Deaths;
            Total = document.Total;
        }
        public string Country {get;set;}
        public int Infections {get;set;}
        public int Recovered {get;set;}
        public int Deaths {get;set;}
        public int Total {get;set;}
    }
}