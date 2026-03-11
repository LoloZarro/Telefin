using MediaBrowser.Controller.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Telefin.Common.Enums;
using Telefin.Common.Models;

namespace Telefin.Helper.Interfaces
{
    public interface IItemQueuesManager
    {
        SemaphoreSlim ProcessingLock { get; }

        ConcurrentDictionary<Guid, QueuedItemContainer> Added { get; }

        ConcurrentDictionary<Guid, QueuedItemContainer> Deleted { get; }

        void AddItemToQueue(NotificationType notificationType, BaseItem item);

        void RemoveItemsFromQueue(NotificationType notificationType, params Guid[] itemIds);

        bool RemoveItemFromAllQueues(Guid itemId);

        QueuedItemContainer[]? DetectMovedItems();

        bool AllQueuesReadyForProcessing();

        bool ItemDeletedQueueReadyForProcessing();

        bool ItemAddedQueueReadyForProcessing();

        void IncrementRetry(IEnumerable<Guid> itemIds);

        bool ShouldRetry(IEnumerable<Guid> itemIds);
    }
}
