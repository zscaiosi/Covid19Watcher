using Covid19Watcher.Application.Enums;

namespace Covid19Watcher.Application.Contracts
{
    public class GetFiltersRequest
    {
        public int Page {get;set;} = 0;
        public int Limit {get;set;} = 20;
        public string Country {get;set;} = string.Empty;
        public bool onlyActives {get;set;}
        public EOrdenation OrderBy {get;set;} = EOrdenation.Deaths;
    }
}