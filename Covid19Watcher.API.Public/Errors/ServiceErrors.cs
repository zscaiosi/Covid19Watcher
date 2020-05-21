using System.ComponentModel;

namespace Covid19Watcher.API.Public.Errors
{
    public enum ServiceErrors
    {
        [Description("Invalid payload.")]
        Post_CreateNotificationAsync_400_Payload = 0,
        [Description("Invalid ID.")]
        GetFind_FindByIdAsync_400_Id,
        [Description("Not found.")]
        Get_ListNotificationsAsync_404_Notification
    }
}