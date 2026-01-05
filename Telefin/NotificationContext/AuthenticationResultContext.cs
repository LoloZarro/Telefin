using System.Collections.Generic;
using System.Globalization;
using MediaBrowser.Controller.Events.Authentication;

namespace Telefin.NotificationContext
{
    internal class AuthenticationResultContext : NotificationContextBase
    {
        private readonly AuthenticationResultEventArgs _eventArgs;

        public AuthenticationResultContext(AuthenticationResultEventArgs eventArgs)
        {
            _eventArgs = eventArgs;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{deviceName}"] = _eventArgs.SessionInfo?.DeviceName;
            data["{username}"] = _eventArgs.User?.Name;
            data["{serverName}"] = _eventArgs.User?.ServerName;
            data["{lastActivity}"] = _eventArgs.User?.LastActivityDate?.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

            return data;
        }
    }
}
