using System.Collections.Generic;
using Jellyfin.Data.Events;
using MediaBrowser.Controller.Session;

namespace Telefin.NotificationContext
{
    internal class AuthenticationRequestContext : NotificationContextBase
    {
        private readonly GenericEventArgs<AuthenticationRequest> _eventArgs;

        public AuthenticationRequestContext(GenericEventArgs<AuthenticationRequest> eventArgs)
        {
            _eventArgs = eventArgs;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{deviceName}"] = _eventArgs.Argument?.DeviceName;
            data["{username}"] = _eventArgs.Argument?.Username;

            return data;
        }
    }
}
