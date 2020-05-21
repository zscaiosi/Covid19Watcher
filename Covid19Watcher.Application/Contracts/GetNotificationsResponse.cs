using Covid19Watcher.Application.Data.MongoDB.Documents;

namespace Covid19Watcher.Application.Contracts
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