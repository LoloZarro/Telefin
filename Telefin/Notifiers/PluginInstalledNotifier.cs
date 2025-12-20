using System;
using System.Threading.Tasks;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Updates;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class PluginInstalledNotifier : IEventConsumer<PluginInstalledEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public PluginInstalledNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(PluginInstalledEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.PluginInstalled, eventArgs).ConfigureAwait(false);
    }
}