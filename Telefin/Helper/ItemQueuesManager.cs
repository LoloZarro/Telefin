using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Model.Configuration;
using Microsoft.Extensions.Logging;
using Telefin.Common.Models;
using Telefin.Helper.Interfaces;
using NotificationType = Telefin.Common.Enums.NotificationType;

namespace Telefin.Helper
{
    internal class ItemQueuesManager : IItemQueuesManager
    {
        private readonly ILogger<ItemQueuesManager> _logger;
        private readonly ILibraryManager _libraryManager;

        public ItemQueuesManager(ILogger<ItemQueuesManager> logger, ILibraryManager libraryManager)
        {
            _logger = logger;
            _libraryManager = libraryManager;
        }

        // TODO: Implement buffer copy solution as this might block the UI if the queues are extremely big
        public SemaphoreSlim ProcessingLock { get; } = new SemaphoreSlim(1, 1);

        public ConcurrentDictionary<Guid, QueuedItemContainer> Added { get; } = new();

        public ConcurrentDictionary<Guid, QueuedItemContainer> Deleted { get; } = new();

        private static int MaxRetries => ConfigurationHelper.GetMetadataWaitMultiplier();

        public bool ItemAddedQueueReadyForProcessing()
        {
            var queueReady = true;

            foreach (var item in Added.ToArray())
            {
                var itemId = item.Key;
                var container = item.Value;

                // Remove invalid items
                var baseItem = _libraryManager.GetItemById(itemId);
                if (baseItem is null)
                {
                    RemoveItemsFromQueue(NotificationType.ItemAdded, itemId);
                    continue;
                }

                // Check if item metadata is present, we need it in order to group the items correctly later.
                if (baseItem.ProviderIds.Count <= 0)
                {
                    if (ShouldRetry([itemId]))
                    {
                        _logger.LogDebug("{PluginName} - {ClassName}: Requeue '{ItemName}(ID: {Id})', no metadata yet (retry attempt {Retry}/{MaxEntries} items)", Plugin.PluginName, nameof(ItemQueuesManager), baseItem?.Name, itemId, item.Value?.RetryCount, MaxRetries);
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

                    _logger.LogWarning("{PluginName} - {ClassName}: Item '{ItemName}(ID: {Id})' has no metadata after {MaxRetries} retries. skipping notification!", Plugin.PluginName, nameof(ItemQueuesManager), baseItem.Name, itemId, MaxRetries);
                    RemoveItemsFromQueue(NotificationType.ItemAdded, itemId);
                    continue;
                }

                container.BaseItem = baseItem;
            }

            return queueReady;
        }

        public bool ItemDeletedQueueReadyForProcessing()
        {
            var queueReady = true;

            foreach (var item in Deleted)
            {
                var itemId = item.Key;
                var container = item.Value;

                if (container is null || container.BaseItem is null)
                {
                    _logger.LogWarning("{PluginName} - {ClassName}: Item {ItemName} cannot be processed. Notification will be skipped for this item.", Plugin.PluginName, nameof(ItemQueuesManager), itemId);
                    RemoveItemsFromQueue(NotificationType.ItemDeleted, itemId);
                    continue;
                }

                // Check if item metadata is present, we need it to resolve placeholders.
                if (container.BaseItem?.ProviderIds?.Count <= 0)
                {
                    _logger.LogWarning("{PluginName} - {ClassName}: Item {ItemName} has no metadata. Notification will be skipped for this item.", Plugin.PluginName, nameof(ItemQueuesManager), container.BaseItem.Name);
                    RemoveItemsFromQueue(NotificationType.ItemDeleted, itemId);
                    continue;
                }
            }

            return queueReady;
        }

        public bool AllQueuesReadyForProcessing()
        {
            return ItemAddedQueueReadyForProcessing() && ItemDeletedQueueReadyForProcessing();
        }

        public void IncrementRetry(IEnumerable<Guid> itemIds)
        {
            foreach (var id in itemIds)
            {
                if (Added.TryGetValue(id, out var c))
                {
                    c.RetryCount++;
                }
            }
        }

        public bool ShouldRetry(IEnumerable<Guid> itemIds)
        {
            // Retry as long as at least one item is under the retry limit
            return itemIds.Any(id =>
                Added.TryGetValue(id, out var container) &&
                container.RetryCount < MaxRetries);
        }

        public void AddItemToQueue(NotificationType notificationType, BaseItem item)
        {
            LibraryOptions options = _libraryManager.GetLibraryOptions(item);

            if (!options.Enabled)
            {
                _logger.LogDebug("{PluginName} - {ClassName}: Not queueing {ItemName} for notification, library is disabled", Plugin.PluginName, nameof(ItemQueuesManager), item.Name);
                return;
            }

            var container = NotificationQueueHelper.CreateContainer(item, _libraryManager);

            bool result;
            switch (notificationType)
            {
                case NotificationType.ItemAdded:
                    result = Added.TryAdd(item.Id, container);
                    break;
                case NotificationType.ItemDeleted:
                    result = Deleted.TryAdd(item.Id, container);
                    break;
                default:
                    return;
            }

            if (result)
            {
                _logger.LogDebug("{PluginName} - {ClassName}: Queued {ItemName} ({Kind}) for notification", Plugin.PluginName, nameof(ItemQueuesManager), item.Name, container.MediaType);
            }
            else
            {
                // Expect duplicate events for the same item.
                _logger.LogDebug("{PluginName} - {ClassName}: Already queued {ItemName} ({Kind}), skipping duplicate", Plugin.PluginName, nameof(ItemQueuesManager), item.Name, container.MediaType);
            }
        }

        public void RemoveItemsFromQueue(NotificationType notificationType, params Guid[] itemIds)
        {
            foreach (var id in itemIds)
            {
                switch (notificationType)
                {
                    case NotificationType.ItemAdded:
                        Added.TryRemove(id, out _);
                        break;
                    case NotificationType.ItemDeleted:
                        Deleted.TryRemove(id, out _);
                        break;
                }
            }
        }

        public bool RemoveItemFromAllQueues(Guid itemId)
        {
            var removed = false;

            removed |= Added.TryRemove(itemId, out _);
            removed |= Deleted.TryRemove(itemId, out _);

            return removed;
        }

        public QueuedItemContainer[]? DetectMovedItems()
        {
            //if (!ConfigurationHelper.SuppressMovedMediaNotifications())
            //{
            //    return null;
            //}

            var matches = new List<QueuedItemContainer>();

            var groupedItemsAdded = NotificationQueueHelper.EvaluateNotificationAddedCandidates(Added.ToArray());

            foreach (QueuedItemContainer item in Deleted.Select(d => d.Value).ToArray())
            {
                var match = groupedItemsAdded.FirstOrDefault(a => NotificationQueueHelper.IsSameItem(item?.BaseItem, a?.BaseItem) >= 8);

                if (match == null)
                {
                    continue;
                }

                matches.Add(match);

                RemoveItemFromAllQueues(item.ItemId);

                RemoveItemFromAllQueues(match.ItemId);
                foreach (var childItemId in match?.ChildItemIds ?? [])
                {
                    RemoveItemFromAllQueues(childItemId);
                }
            }

            return matches.ToArray();
        }
    }
}
