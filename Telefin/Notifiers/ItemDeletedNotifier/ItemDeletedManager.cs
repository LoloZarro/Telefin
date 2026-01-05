using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
using Telefin.Notifiers.ItemAddedNotifier;

namespace Telefin.Notifiers.ItemDeletedNotifier;

public class ItemDeletedManager : IItemDeletedManager
{
    private const NotificationType TypeOfNotification = NotificationType.ItemDeleted;

    private readonly ILogger<ItemDeletedManager> _logger;
    private readonly ILibraryManager _libraryManager;
    private readonly IServerApplicationHost _applicationHost;
    private readonly ConcurrentDictionary<Guid, QueuedItemContainer> _itemProcessQueue; // TODO: Create own object for queue

    public ItemDeletedManager(
        ILogger<ItemDeletedManager> logger,
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
        _logger.LogDebug("{PluginName} - {ClassName}: Processing notification queue for recently deleted items...", Plugin.PluginName, nameof(ItemDeletedManager));

        var queueSnapshot = _itemProcessQueue.ToArray();
        if (queueSnapshot.Length == 0)
        {
            _logger.LogInformation("{PluginName} - {ClassName}: No recently deleted items to process in the queue", Plugin.PluginName, nameof(ItemDeletedManager));
            return;
        }

        var validatedQueue = PrepareQueueItemsForNotification(queueSnapshot);

        _logger.LogInformation("{PluginName} - {ClassName}: {Amount} notification(s) ready to be sent out.", Plugin.PluginName, nameof(ItemAddedManager), validatedQueue.Length);

        var scope = _applicationHost.ServiceProvider!.CreateAsyncScope();
        var notificationDispatcher = scope.ServiceProvider.GetRequiredService<NotificationDispatcher>();

        await using (scope.ConfigureAwait(false))
        {
            foreach (var queueItem in validatedQueue)
            {
                var item = queueItem.BaseItem;
                if (item is null)
                {
                    _logger.LogDebug("{PluginName} - {ClassName}: Item {ItemId} not found, removing from queue", Plugin.PluginName, nameof(ItemAddedManager), queueItem.ItemId);
                    MarkProcessed(queueItem.ItemId);
                    continue;
                }

                _logger.LogDebug("{PluginName} - {ClassName}: Processing notification for {ItemName}", Plugin.PluginName, nameof(ItemDeletedManager), item.Name);

                await notificationDispatcher.DispatchNotificationsAsync(
                    TypeOfNotification,
                    item,
                    userId: string.Empty,
                    subtype: TypeOfNotification.ToNotificationSubType(item)!)
                    .ConfigureAwait(false);

                MarkProcessed(queueItem.ItemId);
            }
        }
    }

    private QueuedItemContainer[] PrepareQueueItemsForNotification(KeyValuePair<Guid, QueuedItemContainer>[] queue)
    {
        var filteredQueue = new List<QueuedItemContainer>();

        foreach (var item in queue)
        {
            var itemId = item.Key;
            var container = item.Value;

            if (container is null)
            {
                continue;
            }

            // Check if item metadata is present yet. We need them to resolve placeholders.
            if (container.BaseItem?.ProviderIds?.Count == 0)
            {
                _logger.LogWarning("{PluginName} - {ClassName}: Item {ItemName} has no metadata. Notification will be skipped for this item.", Plugin.PluginName, nameof(ItemDeletedManager), container.BaseItem.Name);
                MarkProcessed(itemId);
                continue;
            }

            filteredQueue.Add(container);
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

    private void MarkProcessed(Guid itemId)
    {
        _itemProcessQueue.TryRemove(itemId, out _);
    }
}
