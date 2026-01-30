using System.Collections.Generic;
using MediaBrowser.Controller.Events.Updates;

namespace Telefin.NotificationContext
{
    internal sealed class PluginUninstalledContext : NotificationContextBase
    {
        private readonly PluginUninstalledEventArgs _eventArgs;

        public PluginUninstalledContext(PluginUninstalledEventArgs eventArgs)
        {
            _eventArgs = eventArgs;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{itemName}"] = _eventArgs?.Argument?.Name;
            data["{itemVersion}"] = _eventArgs?.Argument?.Version?.ToString();
            data["{itemDescription}"] = _eventArgs?.Argument?.Description;
            data["{itemPackageDescription}"] = _eventArgs?.Argument?.Description;
            data["{itemStatus}"] = _eventArgs?.Argument?.Status.ToString();

            return data;
        }
    }
}
