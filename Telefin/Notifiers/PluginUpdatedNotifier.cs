using System;
using System.Threading.Tasks;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Updates;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class PluginUpdatedNotifier : IEventConsumer<PluginUpdatedEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public PluginUpdatedNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(PluginUpdatedEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.PluginUpdated, eventArgs).ConfigureAwait(false);
    }
}