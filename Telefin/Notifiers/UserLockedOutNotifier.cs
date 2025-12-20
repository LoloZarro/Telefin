using System;
using System.Threading.Tasks;
using Jellyfin.Data.Events.Users;
using MediaBrowser.Controller.Events;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class UserLockedOutNotifier : IEventConsumer<UserLockedOutEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public UserLockedOutNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(UserLockedOutEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.UserLockedOut, eventArgs).ConfigureAwait(false);
    }
}