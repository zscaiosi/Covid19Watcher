using Covid19Watcher.Application.Enums;

namespace Covid19Watcher.Application.Contracts
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