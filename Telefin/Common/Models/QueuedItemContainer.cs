using System;
using System.Collections.Generic;
using MediaBrowser.Controller.Entities;
using Telefin.Common.Enums;

namespace Telefin.Common.Models;

public sealed class QueuedItemContainer
{
    public QueuedItemContainer(Guid itemId, MediaType mediaType, BaseItem? item = null)
    {
        ItemId = itemId;
        MediaType = mediaType;
        RetryCount = 0;
        BaseItem = item;
    }

    public QueuedItemContainer(BaseItem item, MediaType mediaType)
    {
        BaseItem = item;
        ItemId = item.Id;
        MediaType = mediaType;
        RetryCount = 0;
    }

    public Guid ItemId { get; init; }

    public int RetryCount { get; set; }

    public MediaType MediaType { get; init; } = MediaType.Unknown;

    public BaseItem? BaseItem { get; set; }

    public HashSet<Guid> ChildItemIds { get; } = new();

    public void AddChild(Guid id) => ChildItemIds.Add(id);

    public void AddChildren(IEnumerable<Guid> ids)
    {
        foreach (var id in ids ?? [])
        {
            ChildItemIds.Add(id);
        }
    }

    public void RemoveAllChildren()
    {
        foreach (var child in ChildItemIds)
        {
            ChildItemIds.Remove(child);
        }

    }
}
