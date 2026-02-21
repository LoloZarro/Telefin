using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;
using Telefin.Helper;
using Telefin.Helper.Interfaces;

namespace Telefin.Notifiers.ItemAddedNotifier;

public class ItemAddedManager : IItemAddedManager
{
    private const NotificationType TypeOfNotification = NotificationType.ItemAdded;

    private readonly ILogger<ItemAddedManager> _logger;
    private readonly ILibraryManager _libraryManager;
    private readonly IServerApplicationHost _applicationHost;
    private readonly IItemQueuesManager _itemQueuesManager;

    public ItemAddedManager(
        ILogger<ItemAddedManager> logger,
        ILibraryManager libraryManager,
        IServerApplicationHost applicationHost,
        IItemQueuesManager itemQueuesManager)
    {
        _logger = logger;
        _libraryManager = libraryManager;
        _applicationHost = applicationHost;
        _itemQueuesManager = itemQueuesManager;
    }

    private static int MaxRetries => ConfigurationHelper.GetMetadataWaitMultiplier();

    public async Task ProcessItemsAsync()
    {
        _logger.LogDebug("{PluginName} - {ClassName}: Processing notification queue for recently added items...", Plugin.PluginName, nameof(ItemAddedManager));

        if (_itemQueuesManager.Added.IsEmpty)
        {
            _logger.LogInformation("{PluginName} - {ClassName}: No items to process!", Plugin.PluginName, nameof(ItemAddedManager));
            return;
        }

        var notificationCandidates = NotificationQueueHelper.EvaluateNotificationAddedCandidates(_itemQueuesManager.Added.ToArray());

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
                    _logger.LogDebug("{PluginName} - {ClassName}: Item 'ID: {Id}' not found, removing from queue", Plugin.PluginName, nameof(ItemAddedManager), currentCandidateId);
                    _itemQueuesManager.RemoveItemsFromQueue(TypeOfNotification, itemIdsFromQeueBeingProcessed.ToArray());
                    continue;
                }

                _logger.LogDebug("{PluginName} - {ClassName}: Processing notification for {ItemName}(ID: {Id}) (consuming {Count} queued items)", Plugin.PluginName, nameof(ItemAddedManager), item.Name, currentCandidateId, itemIdsFromQeueBeingProcessed.Count);

                if (item.ProviderIds.Count <= 0)
                {
                    if (_itemQueuesManager.ShouldRetry([currentCandidateId]))
                    {
                        _logger.LogDebug("{PluginName} - {ClassName}: Requeue '{ItemName}(ID: {Id})', no metadata yet (retry attempt {Retry}/{MaxEntries} items)", Plugin.PluginName, nameof(ItemAddedManager), item.Name, currentCandidateId, candidate.RetryCount, MaxRetries);
                        _itemQueuesManager.IncrementRetry(itemIdsFromQeueBeingProcessed);
                        continue;
                    }

                    _logger.LogWarning("{PluginName} - {ClassName}: Item '{ItemName}(ID: {Id})' has no metadata after {MaxRetries} retries, skipping notification!", Plugin.PluginName, nameof(ItemAddedManager), item.Name, currentCandidateId, MaxRetries);
                    _itemQueuesManager.RemoveItemsFromQueue(TypeOfNotification, itemIdsFromQeueBeingProcessed.ToArray());
                    continue;
                }

                await notificationDispatcher.DispatchNotificationsAsync(
                    TypeOfNotification,
                    item,
                    userId: string.Empty,
                    subtype: TypeOfNotification.ToNotificationSubType(item)!) // Entry point already checks for null
                    .ConfigureAwait(false);

                _itemQueuesManager.RemoveItemsFromQueue(TypeOfNotification, itemIdsFromQeueBeingProcessed.ToArray());
            }
        }
    }
}
