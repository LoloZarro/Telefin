using System.Collections.Generic;

namespace Telefin.NotificationContext.Interface
{
    public interface INotificationContext
    {
        IDictionary<string, string?> GetTemplateData();

        string? GetImagePath();
    }
}
