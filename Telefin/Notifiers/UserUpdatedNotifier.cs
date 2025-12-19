using System;
using System.Threading.Tasks;
using Jellyfin.Data.Events.Users;
using Telefin.Common.Enums;
using Telefin.Helper;
using MediaBrowser.Controller.Events;

namespace Telefin.Notifiers;

public class UserUpdatedNotifier : IEventConsumer<UserUpdatedEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public UserUpdatedNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(UserUpdatedEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.UserUpdated, eventArgs).ConfigureAwait(false);
    }
}