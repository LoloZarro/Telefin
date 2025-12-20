using System;
using System.Threading.Tasks;
using Jellyfin.Data.Events.System;
using MediaBrowser.Controller.Events;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class PendingRestartNotifier : IEventConsumer<PendingRestartEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public PendingRestartNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(PendingRestartEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.PendingRestart, eventArgs).ConfigureAwait(false);
    }
}