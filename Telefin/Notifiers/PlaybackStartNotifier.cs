using System.Threading.Tasks;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;
using Telefin.Helper;
using Telefin.Notifiers.ItemAddedNotifier;

namespace Telefin.Notifiers;

public class PlaybackStartNotifier : IEventConsumer<PlaybackStartEventArgs>
{
    private const NotificationType TypeOfNotification = NotificationType.PlaybackStart;

    private readonly ILogger<ItemAddedManager> _logger;
    private readonly NotificationDispatcher _notificationDispatcher;

    public PlaybackStartNotifier(ILogger<ItemAddedManager> logger, NotificationDispatcher notificationFilter)
    {
        _logger = logger;
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(PlaybackStartEventArgs eventArgs)
    {
        if (eventArgs?.Item is null || eventArgs.Users.Count == 0 || eventArgs.Item.IsThemeMedia)
        {
            return;
        }

        var subType = TypeOfNotification.ToNotificationSubType(eventArgs.Item);
        if (subType == null)
        {
            _logger.LogWarning("{PluginName}: Notification for media type '{MediaType}' is not supported", typeof(Plugin).Name, eventArgs.Item.GetType().ToString());
            return;
        }

        string userId = eventArgs.Users[0].Id.ToString();

        await _notificationDispatcher.DispatchNotificationsAsync(TypeOfNotification, eventArgs, userId: userId, subtype: subType).ConfigureAwait(false);
    }
}