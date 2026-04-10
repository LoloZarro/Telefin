using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers.MaintenanceNotifier;

public class MaintenanceEndScheduledTask : IScheduledTask, IConfigurableScheduledTask
{
    private const NotificationType TypeOfNotification = NotificationType.MaintenanceEnd;

    private readonly ILogger<MaintenanceEndScheduledTask> _logger;
    private readonly IServerApplicationHost _applicationHost;

    public MaintenanceEndScheduledTask(
        ILogger<MaintenanceEndScheduledTask> logger,
        IServerApplicationHost applicationHost)
    {
        _logger = logger;
        _applicationHost = applicationHost;
    }

    public string Name => "Maintenance End Broadcast";

    public string Key => "TelefinMaintenanceEnd";

    public string Description => "Broadcasts the configured maintenance end message to every enabled user's Telegram chats. Use this to announce the end of a maintenance window.";

    public string Category => "Telefin";

    public bool IsHidden => false;

    public bool IsEnabled => true;

    public bool IsLogged => true;

    public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{PluginName}({NotificationType}): Broadcasting maintenance end notification.", Plugin.PluginName, TypeOfNotification);

        var scope = _applicationHost.ServiceProvider!.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            var dispatcher = scope.ServiceProvider.GetRequiredService<NotificationDispatcher>();
            await dispatcher.DispatchGlobalNotificationAsync(TypeOfNotification, ConfigurationHelper.GetMaintenanceEndMessage()).ConfigureAwait(false);
        }
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        return Array.Empty<TaskTriggerInfo>();
    }
}
