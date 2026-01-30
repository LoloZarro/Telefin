using System.Collections.Generic;
using MediaBrowser.Common.Updates;

namespace Telefin.NotificationContext
{
    internal sealed class PluginInstallationFailedContext : NotificationContextBase
    {
        private readonly InstallationFailedEventArgs _eventArgs;

        public PluginInstallationFailedContext(InstallationFailedEventArgs eventArgs)
        {
            _eventArgs = eventArgs;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{errorMessage}"] = _eventArgs?.Exception.Message;
            data["{itemName}"] = _eventArgs?.InstallationInfo?.Name;
            data["{itemVersion}"] = _eventArgs?.InstallationInfo?.Version?.ToString();
            data["{itemUrl}"] = _eventArgs?.InstallationInfo?.SourceUrl;
            data["{itemPackageName}"] = _eventArgs?.InstallationInfo?.PackageInfo?.Name;
            data["{itemPackageDescription}"] = _eventArgs?.InstallationInfo?.PackageInfo?.Description;
            data["{itemPackageOverview}"] = _eventArgs?.InstallationInfo?.PackageInfo?.Overview;
            data["{itemPackageCategory}"] = _eventArgs?.InstallationInfo?.PackageInfo?.Category;

            return data;
        }
    }
}
