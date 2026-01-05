using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Tasks;

namespace Telefin.Notifiers.ItemDeletedNotifier;

public class ItemDeletedScheduledTask : IScheduledTask, IConfigurableScheduledTask
{
    private const int RecheckIntervalSec = 60;
    private readonly IItemDeletedManager _itemDeletedManager;

    public ItemDeletedScheduledTask(IItemDeletedManager itemDeletedManager)
    {
        _itemDeletedManager = itemDeletedManager;
    }

    public string Name => "Deleted items notifier";

    public string Key => "Telefin";

    public string Description => "Processes all items added to the queue of item that have been removed from the server since the last run, including items still pending, and notifies all configured users subscribed to the relevant events(Episode, Season, etc.)";

    public string Category => "Telefin";

    public bool IsHidden => false;

    public bool IsEnabled => true;

    public bool IsLogged => false;

    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        return _itemDeletedManager.ProcessItemsAsync();
    }

    public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
    {
        return new[]
        {
            new TaskTriggerInfo
            {
                Type = TaskTriggerInfoType.IntervalTrigger,
                IntervalTicks = TimeSpan.FromSeconds(RecheckIntervalSec).Ticks
            }
        };
    }
}
