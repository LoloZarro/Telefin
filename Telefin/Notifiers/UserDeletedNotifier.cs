using System;
using System.Threading.Tasks;
using Jellyfin.Data.Events.Users;
using MediaBrowser.Controller.Events;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class UserDeletedNotifier : IEventConsumer<UserDeletedEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public UserDeletedNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(UserDeletedEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.UserDeleted, eventArgs).ConfigureAwait(false);
    }
}