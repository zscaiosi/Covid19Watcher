using Covid19Watcher.API.Public.Data.MongoDB.Documents;

namespace Covid19Watcher.API.Public.Contracts
{
    public class GetNotificationsResponse : BaseResponse<NotificationDocument>
    {
        public GetNotificationsResponse(int page, int limit)
        {
            Page = page;
            Size = limit;
        }
    }
}