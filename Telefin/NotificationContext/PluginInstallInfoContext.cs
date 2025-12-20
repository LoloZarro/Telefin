using System.Collections.Generic;
using Jellyfin.Data.Events;
using MediaBrowser.Model.Updates;

namespace Telefin.NotificationContext
{
    internal class PluginInstallInfoContext : NotificationContextBase
    {
        private readonly GenericEventArgs<InstallationInfo> _eventArgs;

        public PluginInstallInfoContext(GenericEventArgs<InstallationInfo> eventArgs)
        {
            _eventArgs = eventArgs;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{itemName}"] = _eventArgs?.Argument?.Name;
            data["{itemVersion}"] = _eventArgs?.Argument?.Version?.ToString();
            data["{itemUrl}"] = _eventArgs?.Argument?.SourceUrl;
            data["{itemPackageName}"] = _eventArgs?.Argument?.PackageInfo?.Name;
            data["{itemPackageDescription}"] = _eventArgs?.Argument?.PackageInfo?.Description;
            data["{itemPackageOverview}"] = _eventArgs?.Argument?.PackageInfo?.Overview;
            data["{itemPackageCategory}"] = _eventArgs?.Argument?.PackageInfo?.Category;

            return data;
        }
    }
}
