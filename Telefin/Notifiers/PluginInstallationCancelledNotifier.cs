using System;
using System.Threading.Tasks;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Updates;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class PluginInstallationCancelledNotifier : IEventConsumer<PluginInstallationCancelledEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public PluginInstallationCancelledNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(PluginInstallationCancelledEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.PluginInstallationCancelled, eventArgs).ConfigureAwait(false);
    }
}