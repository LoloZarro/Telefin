using System;
using System.Threading.Tasks;
using Jellyfin.Data.Events.Users;
using Telefin.Common.Enums;
using Telefin.Helper;
using MediaBrowser.Controller.Events;

namespace Telefin.Notifiers;

public class UserPasswordChangedNotifier : IEventConsumer<UserPasswordChangedEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public UserPasswordChangedNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(UserPasswordChangedEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.UserPasswordChanged, eventArgs).ConfigureAwait(false);
    }
}