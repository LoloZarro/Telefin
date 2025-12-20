using System.Collections.Generic;
using Telefin.NotificationContext.Interface;

namespace Telefin.NotificationContext
{
    internal abstract class NotificationContextBase : INotificationContext
    {
        public abstract IDictionary<string, string?> GetTemplateData();

        public virtual string? GetImagePath()
        {
            return null;
        }
    }
}
