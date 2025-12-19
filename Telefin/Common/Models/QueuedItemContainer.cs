using System;
using System.Collections.Generic;
using Telefin.Common.Enums;
using MediaBrowser.Controller.Entities;

namespace Telefin.Common.Models;

public sealed class QueuedItemContainer
{
    public QueuedItemContainer(Guid itemId, MediaSubType kind)
    {
        ItemId = itemId;
        MediaType = kind;
        RetryCount = 0;
    }

    public Guid ItemId { get; init; }

    public int RetryCount { get; set; }

    public MediaSubType MediaType { get; init; } = MediaSubType.Unknown;

    public BaseItem? BaseItem { get; set; }

    public HashSet<Guid> ChildItemIds { get; } = new();

    public void AddChild(Guid id) => ChildItemIds.Add(id);

    public void AddChildren(IEnumerable<Guid> ids)
    {
        foreach (var id in ids)
        {
            ChildItemIds.Add(id);
        }
    }
}
