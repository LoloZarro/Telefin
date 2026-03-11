using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.Tasks;

namespace Telefin.Notifiers.ItemAddedNotifier;

public class LibraryChangedScheduledTask : IScheduledTask, IConfigurableScheduledTask
{
    private const int RecheckIntervalSec = 300
        ;
    private readonly ILibraryChangedManager _libraryChangedManager;

    public LibraryChangedScheduledTask(ILibraryChangedManager itemAddedManager)
    {
        _libraryChangedManager = itemAddedManager;
    }

    public string Name => "Library changed notifier";

    public string Key => "LibraryChangeNotifier";

    public string Description => "Processes all items that have been added or deleted to the server since the last run and notifies all configured users subscribed to the relevant events(Episode, Season, etc.)";

    public string Category => "Telefin";

    public bool IsHidden => false;

    public bool IsEnabled => true;

    public bool IsLogged => false;

    public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        return _libraryChangedManager.ProcessItemsAsync();
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
