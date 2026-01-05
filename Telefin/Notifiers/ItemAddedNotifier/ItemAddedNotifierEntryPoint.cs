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

namespace Telefin.Notifiers.ItemAddedNotifier;

public class ItemAddedNotifierEntryPoint : IHostedService
{
    private const NotificationType TypeOfNotification = NotificationType.ItemAdded;

    private readonly ILogger<ItemAddedNotifierEntryPoint> _logger;
    private readonly IItemAddedManager _itemAddedManager;
    private readonly ILibraryManager _libraryManager;

    public ItemAddedNotifierEntryPoint(
        ILogger<ItemAddedNotifierEntryPoint> logger,
        IItemAddedManager itemAddedManager,
        ILibraryManager libraryManager)
    {
        _logger = logger;
        _itemAddedManager = itemAddedManager;
        _libraryManager = libraryManager;
    }

    private void ItemAddedHandler(object? sender, ItemChangeEventArgs itemChangeEventArgs)
    {
        if (itemChangeEventArgs.UpdateReason != 0) // Do not renotify after initial adding
        {
            return;
        }

        var item = itemChangeEventArgs.Item;

        var subType = TypeOfNotification.ToNotificationSubType(item);
        if (subType == null)
        {
            _logger.LogDebug("{PluginName}: Notification for media type '{MediaType}' is not supported", Plugin.PluginName, item.GetType().ToString());
            return;
        }

        if (!item.IsVirtualItem && item is Movie or Series or Season or Episode or MusicAlbum or Audio or Book) // Audio covers AudioBook
        {
            _itemAddedManager.AddItem(item);
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
}
