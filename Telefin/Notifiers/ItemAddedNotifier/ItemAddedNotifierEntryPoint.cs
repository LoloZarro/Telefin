using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;
using Telefin.Helper.Interfaces;

namespace Telefin.Notifiers.ItemAddedNotifier;

public class ItemAddedNotifierEntryPoint : IHostedService
{
    private const NotificationType TypeOfNotification = NotificationType.ItemAdded;

    private readonly ILogger<ItemAddedNotifierEntryPoint> _logger;
    private readonly IItemQueuesManager _itemQueuesManager;
    private readonly ILibraryManager _libraryManager;

    public ItemAddedNotifierEntryPoint(
        ILogger<ItemAddedNotifierEntryPoint> logger,
        IItemQueuesManager itemQueuesManager,
        ILibraryManager libraryManager)
    {
        _logger = logger;
        _itemQueuesManager = itemQueuesManager;
        _libraryManager = libraryManager;
    }

    private void ItemAddedHandler(object? sender, ItemChangeEventArgs itemChangeEventArgs)
    {
        if (itemChangeEventArgs.UpdateReason != 0) // Do not renotify after initial adding
        {
            return;
        }

        var item = itemChangeEventArgs.Item;

        if (!IsSubTypeSupported(item) || item.IsVirtualItem)
        {
            return;
        }

        if (item is Movie or Series or Season or Episode or MusicAlbum or Audio or Book) // Audio covers AudioBook
        {
            _itemQueuesManager.ProcessingLock.WaitAsync().ConfigureAwait(false);

            try
            {
                _itemQueuesManager.AddItemToQueue(TypeOfNotification, item);
            }
            finally
            {
                _itemQueuesManager.ProcessingLock.Release();
            }
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _libraryManager.ItemAdded += ItemAddedHandler;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _libraryManager.ItemAdded -= ItemAddedHandler;
        return Task.CompletedTask;
    }

    private bool IsSubTypeSupported(BaseItem item)
    {
        var subType = TypeOfNotification.ToNotificationSubType(item);
        if (subType == null)
        {
            _logger.LogDebug("{PluginName} - {ClassName}: Notification for media type '{MediaType}' is not supported", Plugin.PluginName, nameof(ItemAddedNotifierEntryPoint), item.GetType().ToString());
            return false;
        }

        return true;
    }
}
