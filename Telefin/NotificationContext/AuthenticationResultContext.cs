using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Data.Events;
using MediaBrowser.Controller.Authentication;

namespace Telefin.NotificationContext
{
    internal class AuthenticationResultContext : NotificationContextBase
    {
        private readonly GenericEventArgs<AuthenticationResult> _eventArgs;

        public AuthenticationResultContext(GenericEventArgs<AuthenticationResult> eventArgs)
        {
            _eventArgs = eventArgs;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{deviceName}"] = _eventArgs.Argument?.SessionInfo?.DeviceName;
            data["{username}"] = _eventArgs.Argument?.SessionInfo?.UserName;
            data["{serverName}"] = _eventArgs.Argument?.User?.ServerName;
            data["{lastActivity}"] = _eventArgs.Argument?.User?.LastActivityDate?.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

            return data;
        }
    }
}
