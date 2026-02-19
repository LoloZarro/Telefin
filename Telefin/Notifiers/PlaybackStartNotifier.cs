using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;
using Telefin.Common.Models;
using Telefin.Helper;
using Telefin.Notifiers.ItemAddedNotifier;

namespace Telefin.Notifiers;

public class PlaybackStartNotifier : IEventConsumer<PlaybackStartEventArgs>
{
    private const NotificationType TypeOfNotification = NotificationType.PlaybackStart;

    private readonly ILogger<ItemAddedManager> _logger;
    private readonly NotificationDispatcher _notificationDispatcher;

    private static readonly ConcurrentDictionary<Guid, LastPlayedItem> LastPlayedItemByUser = new();

    public PlaybackStartNotifier(ILogger<ItemAddedManager> logger, NotificationDispatcher notificationFilter)
    {
        _logger = logger;
        _notificationDispatcher = notificationFilter;
    }

    private static int DebounceMs => ConfigurationHelper.PlaybackStartDebounceMs();

    public async Task OnEvent(PlaybackStartEventArgs eventArgs)
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

        var userId = eventArgs.Users[0].Id;
        var itemId = eventArgs.Item.Id;

        if (DebounceMs > 0 && GetLastPlayed(userId, out var lastPlayedItem) && lastPlayedItem != null && lastPlayedItem.Id == itemId)
        {
            if (NowMs() - lastPlayedItem.Timestamp <= DebounceMs)
            {
                return;
            }
        }

        SetLastPlayed(userId, itemId);

        await _notificationDispatcher.DispatchNotificationsAsync(TypeOfNotification, eventArgs, userId: userId.ToString(), subtype: subType).ConfigureAwait(false);
    }

    private static long NowMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private void SetLastPlayed(Guid userId, Guid itemId)
    {
        LastPlayedItemByUser[userId] = new LastPlayedItem(itemId, NowMs());
    }

    private bool GetLastPlayed(Guid userId, out LastPlayedItem? state)
    {
        return LastPlayedItemByUser.TryGetValue(userId, out state);
    }
}