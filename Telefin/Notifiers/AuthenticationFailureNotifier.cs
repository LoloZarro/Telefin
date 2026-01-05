using System;
using System.Threading.Tasks;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Authentication;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class AuthenticationFailureNotifier : IEventConsumer<AuthenticationRequestEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public AuthenticationFailureNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(AuthenticationRequestEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.AuthenticationFailure, eventArgs).ConfigureAwait(false);
    }
}