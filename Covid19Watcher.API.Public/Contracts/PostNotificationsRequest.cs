using System;

namespace Covid19Watcher.API.Public.Contracts
{
    public class PostNotificationsRequest
    {
        public string CountryName {get;set;}
        public DateTime CaptureTime {get;set;}
        public bool IsActive {get;set;}
    }
}