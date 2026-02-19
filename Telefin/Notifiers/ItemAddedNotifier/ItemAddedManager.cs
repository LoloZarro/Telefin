using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;
using Telefin.Common.Models;
using Telefin.Helper;

namespace Telefin.Notifiers.ItemAddedNotifier;

public class ItemAddedManager : IItemAddedManager
{
    private const NotificationType TypeOfNotification = NotificationType.ItemAdded;

    private readonly ILogger<ItemAddedManager> _logger;
    private readonly ILibraryManager _libraryManager;
    private readonly IServerApplicationHost _applicationHost;
    private readonly ConcurrentDictionary<Guid, QueuedItemContainer> _itemProcessQueue; // TODO: Create own object for queue

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

    private static int MaxRetries => ConfigurationHelper.GetMetadataWaitMultiplier();

    public async Task ProcessItemsAsync()
    {
        _logger.LogDebug("{PluginName} - {ClassName}: Processing notification queue for recently added items...", Plugin.PluginName, nameof(ItemAddedManager));

        var queueSnapshot = _itemProcessQueue.ToArray();
        if (queueSnapshot.Length == 0)
        {
            _logger.LogInformation("{PluginName} - {ClassName}: No items to process", Plugin.PluginName, nameof(ItemAddedManager));
            return;
        }

        var validatedQueue = PrepareQueueItemsForNotification(queueSnapshot);
        if (validatedQueue is null)
        {
            _logger.LogWarning("{PluginName} - {ClassName}: Not all items in the queue are ready to be processed, retrying next run.", Plugin.PluginName, nameof(ItemAddedManager));
            return;
        }

        var notificationCandidates = NotificationQueueHelper.EvaluateNotificationCandidates(validatedQueue);

        _logger.LogInformation("{PluginName} - {ClassName}: {Amount} notification(s) ready to be sent out.", Plugin.PluginName, nameof(ItemAddedManager), notificationCandidates.Count);

        var scope = _applicationHost.ServiceProvider!.CreateAsyncScope();
        var notificationDispatcher = scope.ServiceProvider.GetRequiredService<NotificationDispatcher>();

        await using (scope.ConfigureAwait(false))
        {
            foreach (var candidate in notificationCandidates)
            {
                var currentCandidateId = candidate.ItemId;
                var itemIdsFromQeueBeingProcessed = candidate.ChildItemIds.Append(currentCandidateId).ToHashSet();

                // BaseItem from the library will be present unless the container was created during evaluation of notification candidates (grouping)
                var item = candidate.BaseItem ?? _libraryManager.GetItemById(currentCandidateId);

                if (item is null)
                {
                    _logger.LogDebug("{PluginName} - {ClassName}: Item {ItemId} not found, removing from queue", Plugin.PluginName, nameof(ItemAddedManager), currentCandidateId);
                    MarkProcessed(itemIdsFromQeueBeingProcessed);
                    continue;
                }

                _logger.LogDebug("{PluginName} - {ClassName}: Processing notification for {ItemName} (consuming {Count} queued items)", Plugin.PluginName, nameof(ItemAddedManager), item.Name, itemIdsFromQeueBeingProcessed.Count);

                if (item.ProviderIds.Count <= 0)
                {
                    if (ShouldRetry([item.Id]))
                    {
                        _logger.LogDebug("{PluginName} - {ClassName}: Requeue {ItemName}, no metadata yet (retry attempt for one of {Count} items)", Plugin.PluginName, nameof(ItemAddedManager), item.Name, itemIdsFromQeueBeingProcessed.Count);
                        IncrementRetry(itemIdsFromQeueBeingProcessed);
                        continue;
                    }

                    _logger.LogWarning("{PluginName} - {ClassName}: Item {ItemName} has no metadata after {MaxRetries} retries; skipping notification", Plugin.PluginName, nameof(ItemAddedManager), item.Name, MaxRetries);
                    MarkProcessed(itemIdsFromQeueBeingProcessed);
                    continue;
                }

                await notificationDispatcher.DispatchNotificationsAsync(
                    TypeOfNotification,
                    item,
                    userId: string.Empty,
                    subtype: TypeOfNotification.ToNotificationSubType(item)!) // Entry point already checks for null
                    .ConfigureAwait(false);

                MarkProcessed(itemIdsFromQeueBeingProcessed);
            }
        }
    }

    private KeyValuePair<Guid, QueuedItemContainer>[]? PrepareQueueItemsForNotification(KeyValuePair<Guid, QueuedItemContainer>[] queue)
    {
        var filteredQueue = new List<KeyValuePair<Guid, QueuedItemContainer>>();
        var queueReady = true;

        foreach (var item in queue)
        {
            var itemId = item.Key;
            var container = item.Value;

            // Remove invalid items
            var baseItem = _libraryManager.GetItemById(itemId);
            if (baseItem is null)
            {
                _logger.LogDebug("{PluginName} - {ClassName}: Item {ItemId} not found, removing from queue", Plugin.PluginName, nameof(ItemAddedManager), itemId);
                MarkProcessed([itemId]);
                continue;
            }

            // Check if item metadata is present, we need it in order to group the items correctly later.
            if (baseItem.ProviderIds.Count <= 0)
            {
                if (ShouldRetry([itemId]))
                {
                    _logger.LogDebug("{PluginName} - {ClassName}: Requeue {ItemName}, no metadata yet. Retry next run!", Plugin.PluginName, nameof(ItemAddedManager), baseItem.Name);
                    IncrementRetry([itemId]);

                    // All items need to have valid metadata before processing the queue as a whole.
                    // This is neccessary to prevent duplicate notification for the same item.
                    //
                    // Example:
                    // A series was uploaded but one of the seasons didn't have providers yet, so it will be skipped.
                    // The next run it will produce a notification for the season that didn't have metadata previously.
                    queueReady = false;
                    continue;
                }

                _logger.LogWarning("{PluginName} - {ClassName}: Item {ItemName} has no metadata after {MaxRetries} retries. Notification will be skipped for this item.", Plugin.PluginName, nameof(ItemAddedManager), baseItem.Name, MaxRetries);
                MarkProcessed([itemId]);
                continue;
            }

            container.BaseItem = baseItem;
            filteredQueue.Add(item);
        }

        return queueReady ? filteredQueue.ToArray() : null;
    }

    public void AddItem(BaseItem item)
    {
        LibraryOptions options = _libraryManager.GetLibraryOptions(item);

        if (!options.Enabled)
        {
            _logger.LogDebug("{PluginName}: Not queueing {ItemName} for notification, library is disabled", Plugin.PluginName, item.Name);
            return;
        }

        var container = NotificationQueueHelper.CreateContainer(item, _libraryManager);

        if (_itemProcessQueue.TryAdd(item.Id, container))
        {
            _logger.LogDebug("{PluginName}: Queued {ItemName} ({Kind}) for notification", Plugin.PluginName, item.Name, container.MediaType);
        }
        else
        {
            // Expect duplicate events for the same item.
            _logger.LogDebug("{PluginName}: Already queued {ItemName} ({Kind}), skipping duplicate", Plugin.PluginName, item.Name, container.MediaType);
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
}
