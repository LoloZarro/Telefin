using System;

namespace Telefin.Common.Models
{
    public class LastPlayedItem()
    {
        public LastPlayedItem(Guid id, long timestamp) : this()
        {
            Id = id;
            Timestamp = timestamp;
        }

        public Guid Id { get; set; }

        public long Timestamp { get; set; }
    }
}
