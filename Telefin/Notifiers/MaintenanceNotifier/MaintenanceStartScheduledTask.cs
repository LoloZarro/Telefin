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

public class MaintenanceStartScheduledTask : IScheduledTask, IConfigurableScheduledTask
{
    private const NotificationType TypeOfNotification = NotificationType.MaintenanceStart;

    private readonly ILogger<MaintenanceStartScheduledTask> _logger;
    private readonly IServerApplicationHost _applicationHost;

    public MaintenanceStartScheduledTask(
        ILogger<MaintenanceStartScheduledTask> logger,
        IServerApplicationHost applicationHost)
    {
        _logger = logger;
        _applicationHost = applicationHost;
    }

    public string Name => "Maintenance Start Broadcast";

    public string Key => "TelefinMaintenanceStart";

    public string Description => "Broadcasts the configured maintenance start message to every enabled user's Telegram chats. Use this to announce planned downtime.";

    public string Category => "Telefin";

    public bool IsHidden => false;

    public bool IsEnabled => true;

    public bool IsLogged => true;

    public async Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{PluginName}({NotificationType}): Broadcasting maintenance start notification.", Plugin.PluginName, TypeOfNotification);

        var scope = _applicationHost.ServiceProvider!.CreateAsyncScope();
        await using (scope.ConfigureAwait(false))
        {
            var dispatcher = scope.ServiceProvider.GetRequiredService<NotificationDispatcher>();
            await dispatcher.DispatchGlobalNotificationAsync(TypeOfNotification, ConfigurationHelper.GetMaintenanceStartMessage()).ConfigureAwait(false);
        }
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        return Array.Empty<TaskTriggerInfo>();
    }
}
