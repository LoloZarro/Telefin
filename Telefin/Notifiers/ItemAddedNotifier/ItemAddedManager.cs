using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Common.Models;
using Telefin.Helper;

namespace Telefin.Notifiers.ItemAddedNotifier;

public class ItemAddedManager : IItemAddedManager
{
    private const int MaxRetries = 10;

    private readonly ILogger<ItemAddedManager> _logger;
    private readonly ILibraryManager _libraryManager;
    private readonly IServerApplicationHost _applicationHost;
    private readonly ConcurrentDictionary<Guid, QueuedItemContainer> _itemProcessQueue;

    public ItemAddedManager(
        ILogger<ItemAddedManager> logger,
        ILibraryManager libraryManager,
        IServerApplicationHost applicationHost)
    {
        _logger = logger;
        _libraryManager = libraryManager;
        _applicationHost = applicationHost;
        _itemProcessQueue = new ConcurrentDictionary<Guid, QueuedItemContainer>();
    }

    public async Task ProcessItemsAsync()
    {
        _logger.LogDebug("{PluginName}: Processing notification queue for recently added items...", typeof(Plugin).Name);

        var queueSnapshot = _itemProcessQueue.ToArray();
        if (queueSnapshot.Length == 0)
        {
            _logger.LogInformation("{PluginName}: No recently added items to process in the queue", typeof(Plugin).Name);
            return;
        }

        var validatedQueue = PrepareQueueItemsForNotification(queueSnapshot);
        var notificationCandidates = NotificationQueueHelper.EvaluateNotificationCandidates(validatedQueue);

        var scope = _applicationHost.ServiceProvider!.CreateAsyncScope();
        var notificationDispatcher = scope.ServiceProvider.GetRequiredService<NotificationDispatcher>();

        await using (scope.ConfigureAwait(false))
        {
            foreach (var candidate in notificationCandidates)
            {
                var itemIdToNotifyOn = candidate.ItemId;
                var sourceIds = candidate.ChildItemIds.Append(itemIdToNotifyOn).ToHashSet(); // all queue items this notification consumes

                // Item from the library will be present unless the container was created during evaluation of notification candidates (grouping)
                var item = candidate.BaseItem ?? _libraryManager.GetItemById(itemIdToNotifyOn);
                if (item is null) // Should technically not be possible anymore
                {
                    _logger.LogDebug("{PluginName}: Item {ItemId} not found, removing from queue", typeof(Plugin).Name, itemIdToNotifyOn);
                    MarkProcessed(sourceIds); // Drop all items related to this candidate
                    continue;
                }

                _logger.LogDebug("{PluginName}: Processing notification for {ItemName} (consuming {Count} queued items)", typeof(Plugin).Name, item.Name, sourceIds.Count);

                if (item.ProviderIds.Keys.Count == 0) // Should technically not be possible anymore
                {
                    if (ShouldRetry(sourceIds))
                    {
                        _logger.LogDebug("{PluginName}: Requeue {ItemName}, no metadata yet (retry attempt for one of {Count} items)", typeof(Plugin).Name, item.Name, sourceIds.Count);
                        IncrementRetry(sourceIds);
                        continue;
                    }

                    _logger.LogWarning("{PluginName}: Item {ItemName} has no metadata after {MaxRetries} retries; skipping notification", typeof(Plugin).Name, item.Name, MaxRetries);
                    MarkProcessed(sourceIds); // Drop all items related to this candidate
                    continue;
                }

                var subtype = GetItemAddedNotificationSubtype(item);
                var imagePath = GetImagePathForItem(item);

                await notificationDispatcher.DispatchNotificationsAsync(
                    NotificationType.ItemAdded,
                    item,
                    userId: string.Empty,
                    imagePath: imagePath,
                    subtype: subtype)
                    .ConfigureAwait(false);

                MarkProcessed(sourceIds);
            }
        }
    }

    private KeyValuePair<Guid, QueuedItemContainer>[] PrepareQueueItemsForNotification(KeyValuePair<Guid, QueuedItemContainer>[] queue)
    {
        var filteredQueue = new List<KeyValuePair<Guid, QueuedItemContainer>>();

        foreach (var item in queue)
        {
            var itemId = item.Key;
            var container = item.Value;

            // Remove invalid items
            var baseItem = _libraryManager.GetItemById(itemId);
            if (baseItem is null)
            {
                _logger.LogDebug("{PluginName}: Item {ItemId} not found, removing from queue", typeof(Plugin).Name, itemId);
                MarkProcessed([itemId]);
                continue;
            }

            // Check if item metadata is present yet. We need it in order to group the items correctly later.
            if (baseItem.ProviderIds.Count == 0)
            {
                if (ShouldRetry([itemId]))
                {
                    _logger.LogDebug("{PluginName}: Requeue {ItemName}, no metadata yet retry next run", typeof(Plugin).Name, baseItem.Name);
                    IncrementRetry([itemId]);
                    continue;
                }

                _logger.LogWarning("{PluginName}: Item {ItemName} has no metadata after {MaxRetries} retries. Notification will be skipped for this item.", typeof(Plugin).Name, baseItem.Name, MaxRetries);
                MarkProcessed([itemId]);
                continue;
            }

            container.BaseItem = baseItem; // Attach the fully featured item from the library
            filteredQueue.Add(item);
        }

        return filteredQueue.ToArray();
    }

    public void AddItem(BaseItem item)
    {
        LibraryOptions options = _libraryManager.GetLibraryOptions(item);

        if (!options.Enabled)
        {
            _logger.LogDebug("Not queueing {ItemName} for notification - library is disabled", item.Name);
            return;
        }

        var container = NotificationQueueHelper.CreateContainer(item, _libraryManager);

        if (_itemProcessQueue.TryAdd(item.Id, container))
        {
            _logger.LogDebug("Queued {ItemName} ({Kind}) for notification", item.Name, container.MediaType);
        }
        else
        {
            // Expect duplicate events for same item.
            _logger.LogDebug("Already queued {ItemName} ({Kind}) - skipping duplicate", item.Name, container.MediaType);
        }
    }

    private void MarkProcessed(IEnumerable<Guid> itemIds)
    {
        foreach (var id in itemIds)
        {
            _itemProcessQueue.TryRemove(id, out _);
        }
    }

    private void IncrementRetry(IEnumerable<Guid> itemIds)
    {
        foreach (var id in itemIds)
        {
            if (_itemProcessQueue.TryGetValue(id, out var c))
            {
                c.RetryCount++;
            }
        }
    }

    private bool ShouldRetry(IEnumerable<Guid> itemIds)
    {
        // Retry as long as at least one item is under the retry limit
        return itemIds.Any(id =>
            _itemProcessQueue.TryGetValue(id, out var container) &&
            container.RetryCount < MaxRetries);
    }

    private static string BuildImageUrl(Guid itemId)
    {
        var raw = Plugin.Instance?.Configuration.ServerUrl ?? "localhost:8096";

        var hasScheme = raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                     || raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

        var baseUri = new Uri(hasScheme ? raw : $"http://{raw}", UriKind.Absolute); // Do not remove scheme if present, could lead to issues

        return new Uri(baseUri, $"/Items/{itemId}/Images/Primary").ToString();
    }

    private static string GetImagePathForItem(BaseItem item)
    {
        if (item is Episode episode)
        {
            return BuildImageUrl(episode.SeriesId);
        }

        if (item.PrimaryImagePath is not null)
        {
            return BuildImageUrl(item.Id);
        }

        return string.Empty;
    }

    private static string GetItemAddedNotificationSubtype(BaseItem item)
    {
        return item switch
        {
            Movie => "ItemAddedMovies",
            Series => "ItemAddedSeries",
            Season => "ItemAddedSeasons",
            Episode => "ItemAddedEpisodes",
            MusicAlbum => "ItemAddedAlbums",
            Audio => "ItemAddedSongs",
            Book => "ItemAddedBooks",
            _ => "ItemAddedMovies"
        };
    }
}
