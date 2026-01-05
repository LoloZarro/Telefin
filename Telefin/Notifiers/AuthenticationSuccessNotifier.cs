using System;
using System.Threading.Tasks;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Events.Authentication;
using Telefin.Common.Enums;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class AuthenticationSuccessNotifier : IEventConsumer<AuthenticationResultEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public AuthenticationSuccessNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(AuthenticationResultEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.AuthenticationSuccess, eventArgs).ConfigureAwait(false);
    }
}