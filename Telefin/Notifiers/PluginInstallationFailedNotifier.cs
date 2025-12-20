using System;
using System.Threading.Tasks;
using MediaBrowser.Common.Updates;
using MediaBrowser.Controller.Events;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class PluginInstallationFailedNotifier : IEventConsumer<InstallationFailedEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public PluginInstallationFailedNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(InstallationFailedEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.PluginInstallationFailed, eventArgs).ConfigureAwait(false);
    }
}