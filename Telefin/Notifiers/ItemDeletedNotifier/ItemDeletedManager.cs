using System.Linq;
using System.Threading.Tasks;
using MediaBrowser.Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;
using Telefin.Helper;
using Telefin.Helper.Interfaces;
using Telefin.Notifiers.ItemAddedNotifier;

namespace Telefin.Notifiers.ItemDeletedNotifier;

public class ItemDeletedManager : IItemDeletedManager
{
    private const NotificationType TypeOfNotification = NotificationType.ItemDeleted;

    private readonly ILogger<ItemDeletedManager> _logger;
    private readonly IServerApplicationHost _applicationHost;
    private readonly IItemQueuesManager _itemQueuesManager;

    public ItemDeletedManager(
        ILogger<ItemDeletedManager> logger,
        IServerApplicationHost applicationHost,
        IItemQueuesManager itemQueuesManager)
    {
        _logger = logger;
        _applicationHost = applicationHost;
        _itemQueuesManager = itemQueuesManager;
    }

    public async Task ProcessItemsAsync()
    {
        _logger.LogDebug("{PluginName} - {ClassName}: Processing notification queue for recently deleted items...", Plugin.PluginName, nameof(ItemDeletedManager));

        if (_itemQueuesManager.Deleted.IsEmpty)
        {
            _logger.LogInformation("{PluginName} - {ClassName}: No items to process!", Plugin.PluginName, nameof(ItemDeletedManager));
            return;
        }

        var deletedQueue = _itemQueuesManager.Deleted.Select(x => x.Value).ToList();

        _logger.LogInformation("{PluginName} - {ClassName}: {Amount} notification(s) ready to be sent out.", Plugin.PluginName, nameof(ItemDeletedManager), deletedQueue.Count);

        var scope = _applicationHost.ServiceProvider!.CreateAsyncScope();
        var notificationDispatcher = scope.ServiceProvider.GetRequiredService<NotificationDispatcher>();

        await using (scope.ConfigureAwait(false))
        {
            foreach (var queueItem in deletedQueue)
            {
                var item = queueItem.BaseItem;

                if (item is null)
                {
                    _logger.LogDebug("{PluginName} - {ClassName}: Item 'ID: {Id}' not found, removing from queue", Plugin.PluginName, nameof(ItemDeletedManager), queueItem.ItemId);
                    _itemQueuesManager.RemoveItemsFromQueue(TypeOfNotification, queueItem.ItemId);
                    continue;
                }

                _logger.LogDebug("{PluginName} - {ClassName}: Processing notification for {ItemName}(ID: {Id})", Plugin.PluginName, nameof(ItemDeletedManager), item.Name, item.Id);

                await notificationDispatcher.DispatchNotificationsAsync(
                    TypeOfNotification,
                    item,
                    userId: string.Empty,
                    subtype: TypeOfNotification.ToNotificationSubType(item)!)
                    .ConfigureAwait(false);

                _itemQueuesManager.RemoveItemsFromQueue(TypeOfNotification, queueItem.ItemId);
            }
        }
    }
}
