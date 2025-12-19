using System;
using System.Threading.Tasks;
using Jellyfin.Data.Events;
using Telefin.Common.Enums;
using Telefin.Helper;
using MediaBrowser.Controller.Authentication;
using MediaBrowser.Controller.Events;

namespace Telefin.Notifiers;

public class AuthenticationSuccessNotifier : IEventConsumer<GenericEventArgs<AuthenticationResult>>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public AuthenticationSuccessNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(GenericEventArgs<AuthenticationResult> eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.AuthenticationSuccess, eventArgs).ConfigureAwait(false);
    }
}