using System.Collections.Generic;

namespace Covid19Watcher.Application.Contracts
{
    public class BaseResponse<T>
    {
        public int Page {get;set;}
        public int Size {get;set;}
        public List<T> Content {get;set;}
    }
}