using Covid19Watcher.Application.Data.MongoDB.Documents;

namespace Covid19Watcher.Application.Contracts
{
    public class GroupedNotifications
    {
        public string Country {get;set;}
        public decimal InfectedRation {get;set;}
        public decimal RecoveredRation {get;set;}
        public decimal DeathsRation {get;set;}
        public NotificationDocument LastNotification {get;set;}
    }
}