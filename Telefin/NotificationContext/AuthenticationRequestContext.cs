using System.Collections.Generic;
using MediaBrowser.Controller.Events.Authentication;

namespace Telefin.NotificationContext
{
    internal class AuthenticationRequestContext : NotificationContextBase
    {
        private readonly AuthenticationRequestEventArgs _eventArgs;

        public AuthenticationRequestContext(AuthenticationRequestEventArgs eventArgs)
        {
            _eventArgs = eventArgs;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{deviceName}"] = _eventArgs.DeviceName;
            data["{username}"] = _eventArgs.Username;

            return data;
        }
    }
}
