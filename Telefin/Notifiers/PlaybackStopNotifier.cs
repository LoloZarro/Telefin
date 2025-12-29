using System.Threading.Tasks;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;
using Telefin.Helper;

namespace Telefin.Notifiers;

public class PlaybackStopNotifier : IEventConsumer<PlaybackStopEventArgs>
{
    private const NotificationType TypeOfNotification = NotificationType.PlaybackStop;

    private readonly ILogger<PlaybackStopNotifier> _logger;
    private readonly NotificationDispatcher _notificationDispatcher;

    public PlaybackStopNotifier(ILogger<PlaybackStopNotifier> logger, NotificationDispatcher notificationFilter)
    {
        _logger = logger;
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(PlaybackStopEventArgs eventArgs)
    {
        if (eventArgs?.Item is null || eventArgs.Users.Count == 0 || eventArgs.Item.IsThemeMedia)
        {
            return;
        }

        var subType = TypeOfNotification.ToNotificationSubType(eventArgs.Item);
        if (subType == null)
        {
            _logger.LogWarning("{PluginName}: Notification for media type '{MediaType}' is not supported", Plugin.PluginName, eventArgs.Item.GetType().ToString());
            return;
        }

        string userId = eventArgs.Users[0].Id.ToString();

        await _notificationDispatcher.DispatchNotificationsAsync(TypeOfNotification, eventArgs, userId: userId, subtype: subType).ConfigureAwait(false);
    }
}