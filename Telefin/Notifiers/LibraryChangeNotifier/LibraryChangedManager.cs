using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telefin.Helper;
using Telefin.Helper.Interfaces;
using Telefin.Notifiers.ItemDeletedNotifier;

namespace Telefin.Notifiers.ItemAddedNotifier;

public class LibraryChangedManager : ILibraryChangedManager
{
    private readonly ILogger<LibraryChangedManager> _logger;
    private readonly IItemAddedManager _itemAddedManager;
    private readonly IItemDeletedManager _itemDeletedManager;
    private readonly IItemQueuesManager _itemQueuesManager;

    public LibraryChangedManager(
        ILogger<LibraryChangedManager> logger,
        IItemAddedManager itemAddedManager,
        IItemDeletedManager itemDeletedManager,
        IItemQueuesManager itemQueuesManager)
    {
        _logger = logger;
        _itemAddedManager = itemAddedManager;
        _itemDeletedManager = itemDeletedManager;
        _itemQueuesManager = itemQueuesManager;
    }

    public async Task ProcessItemsAsync()
    {
        await _itemQueuesManager.ProcessingLock.WaitAsync().ConfigureAwait(false);

        try
        {
            if (ConfigurationHelper.SuppressMovedMediaNotifications())
            {
                _logger.LogDebug("{PluginName} - {ClassName}: Detection of moved items is enabled.", Plugin.PluginName, nameof(LibraryChangedManager));

                if (!_itemQueuesManager.AllQueuesReadyForProcessing())
                {
                    _logger.LogWarning("{PluginName} - {ClassName}: Not all items in the queues are ready to be processed, retrying next run.", Plugin.PluginName, nameof(LibraryChangedManager));
                    return;
                }

                var detectedMoves = _itemQueuesManager.DetectMovedItems();

                if (detectedMoves?.Length > 0)
                {
                    _logger.LogInformation("{PluginName} - {ClassName}: {DetectedMoves} move(s) have been detected, notification will not be sent for these items. Enable debug log level to see the details.", Plugin.PluginName, nameof(ItemAddedManager), detectedMoves.Length);
                    _logger.LogDebug("{PluginName} - {ClassName}: Detected moved item(s) -> {Json}", Plugin.PluginName, nameof(LibraryChangedManager), JsonSerializer.Serialize(detectedMoves));
                }

                await _itemAddedManager.ProcessItemsAsync().ConfigureAwait(true);
                await _itemDeletedManager.ProcessItemsAsync().ConfigureAwait(true);

                return;
            }

            if (_itemQueuesManager.ItemAddedQueueReadyForProcessing())
            {
                await _itemAddedManager.ProcessItemsAsync().ConfigureAwait(true);
            }

            if (_itemQueuesManager.ItemDeletedQueueReadyForProcessing())
            {
                await _itemDeletedManager.ProcessItemsAsync().ConfigureAwait(true);
            }
        }
        finally
        {
            _itemQueuesManager.ProcessingLock.Release();
        }
    }
}
