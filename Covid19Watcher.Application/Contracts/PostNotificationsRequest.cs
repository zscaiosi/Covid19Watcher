using System;

namespace Covid19Watcher.Application.Contracts
{
    public class PostNotificationsRequest
    {
        public string CountryName {get;set;}
        public DateTime CaptureTime {get;set;}
        public bool IsActive {get;set;}
        public int Infections {get;set;} = 0;
        public int Deaths {get;set;} = 0;
        public int Recovered {get;set;} = 0;
    }
}