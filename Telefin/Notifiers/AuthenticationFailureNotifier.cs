using System;
using System.Threading.Tasks;
using Jellyfin.Data.Events;
using Telefin.Common.Enums;
using Telefin.Helper;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Session;

namespace Telefin.Notifiers;

public class AuthenticationFailureNotifier : IEventConsumer<GenericEventArgs<AuthenticationRequest>>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public AuthenticationFailureNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(GenericEventArgs<AuthenticationRequest> eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.AuthenticationFailure, eventArgs).ConfigureAwait(false);
    }
}