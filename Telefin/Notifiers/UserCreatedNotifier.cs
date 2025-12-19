using System;
using System.Threading.Tasks;
using Jellyfin.Data.Events.Users;
using Telefin.Common.Enums;
using Telefin.Helper;
using MediaBrowser.Controller.Events;

namespace Telefin.Notifiers;

public class UserCreatedNotifier : IEventConsumer<UserCreatedEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public UserCreatedNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(UserCreatedEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.UserCreated, eventArgs).ConfigureAwait(false);
    }
}