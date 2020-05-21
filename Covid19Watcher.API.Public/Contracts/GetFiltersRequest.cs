using Covid19Watcher.API.Public.Enums;

namespace Covid19Watcher.API.Public.Contracts
{
    public class GetFiltersRequest
    {
        public int Page {get;set;}
        public int Limit {get;set;}
        public string Country {get;set;}
        public bool onlyActives {get;set;}
        public EOrdenation OrderBy {get;set;} = EOrdenation.Deaths;
    }
}