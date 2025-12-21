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

namespace Telefin.Notifiers.ItemDeletedNotifier;

public class ItemDeletedNotifierEntryPoint : IHostedService
{
    private const NotificationType TypeOfNotification = NotificationType.ItemDeleted;

    private readonly ILogger<ItemDeletedNotifierEntryPoint> _logger;
    private readonly IItemDeletedManager _itemDeletedManager;
    private readonly ILibraryManager _libraryManager;

    public ItemDeletedNotifierEntryPoint(
        ILogger<ItemDeletedNotifierEntryPoint> logger,
        IItemDeletedManager itemAddedManager,
        ILibraryManager libraryManager)
    {
        _logger = logger;
        _itemDeletedManager = itemAddedManager;
        _libraryManager = libraryManager;
    }

    private void ItemDeletedHandler(object? sender, ItemChangeEventArgs itemChangeEventArgs)
    {
        if (itemChangeEventArgs.UpdateReason != 0) // Do not renotify after initial adding
        {
            return;
        }

        var item = itemChangeEventArgs.Item;

        if (AddItemToQueue(item))
        {
            return;
        }

        // Fallback: check for folder type (deleted episodes are delievered in a folder for example)
        if (item is Folder folder)
        {
            foreach (var child in folder.Children ?? [])
            {
                AddItemToQueue(child);
            }
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _libraryManager.ItemRemoved += ItemDeletedHandler;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _libraryManager.ItemRemoved -= ItemDeletedHandler;
        return Task.CompletedTask;
    }

    private bool AddItemToQueue(BaseItem item)
    {
        // Never notify on virtual items.
        if (!IsSubTypeSupported(item) && !item.IsVirtualItem)
        {
            return false;
        }

        if (item is Movie or Series or Season or Episode or MusicAlbum or Audio or Book) // Audio covers AudioBook
        {
            _itemDeletedManager.AddItem(item);
            return true;
        }

        return false;
    }

    private bool IsSubTypeSupported(BaseItem item)
    {
        var subType = TypeOfNotification.ToNotificationSubType(item);
        if (subType == null)
        {
            _logger.LogWarning("{PluginName}: Notification for media type '{MediaType}' is not supported", typeof(Plugin).Name, item.GetType().ToString());
            return false;
        }

        return true;
    }
}
