using System;
using System.Threading.Tasks;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Updates;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class PluginUninstalledNotifier : IEventConsumer<PluginUninstalledEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public PluginUninstalledNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(PluginUninstalledEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.PluginUninstalled, eventArgs).ConfigureAwait(false);
    }
}