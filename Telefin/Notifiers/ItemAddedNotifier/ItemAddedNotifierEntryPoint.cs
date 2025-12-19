using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Hosting;

namespace Telefin.Notifiers.ItemAddedNotifier;

public class ItemAddedNotifierEntryPoint : IHostedService
{
    private readonly IItemAddedManager _itemAddedManager;
    private readonly ILibraryManager _libraryManager;

    public ItemAddedNotifierEntryPoint(
        IItemAddedManager itemAddedManager,
        ILibraryManager libraryManager)
    {
        _itemAddedManager = itemAddedManager;
        _libraryManager = libraryManager;
    }

    private void ItemAddedHandler(object? sender, ItemChangeEventArgs itemChangeEventArgs)
    {
        var item = itemChangeEventArgs.Item;

        // Never notify on virtual items.
        if (!item.IsVirtualItem && item is Movie or Series or Season or Episode or MusicAlbum or Audio or Book or AudioBook)
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
