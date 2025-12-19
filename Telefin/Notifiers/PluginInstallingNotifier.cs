using System;
using System.Threading.Tasks;
using Telefin.Common.Enums;
using Telefin.Helper;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Updates;

namespace Telefin.Notifiers;

public class PluginInstallingNotifier : IEventConsumer<PluginInstallingEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public PluginInstallingNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(PluginInstallingEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.PluginInstalling, eventArgs).ConfigureAwait(false);
    }
}