using System;
using System.Threading.Tasks;
using Telefin.Common.Enums;
using Telefin.Helper;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Updates;

namespace Telefin.Notifiers;

public class PluginUpdatedNotifier : IEventConsumer<PluginUpdatedEventArgs>
{
    private readonly NotificationDispatcher notificationDispatcher;

    public PluginUpdatedNotifier(NotificationDispatcher notificationFilter)
    {
        notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(PluginUpdatedEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await notificationDispatcher.DispatchNotificationsAsync(NotificationType.PluginUpdated, eventArgs).ConfigureAwait(false);
    }
}