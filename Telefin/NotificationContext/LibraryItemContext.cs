using System.Collections.Generic;
using MediaBrowser.Controller.Entities;
using Telefin.Common.Extensions;

namespace Telefin.NotificationContext
{
    internal class LibraryItemContext : NotificationContextBase
    {
        private readonly BaseItem _item;

        public LibraryItemContext(BaseItem item)
        {
            _item = item;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{itemTitle}"] = _item.GetPropertySafely<string?>("Name");
            data["{itemYear}"] = _item.GetPropertySafely<string?>("ProductionYear");
            data["{mediaType}"] = _item.GetPropertySafely<string?>("MediaType");
            data["{itemOverview}"] = _item.GetPropertySafely<string?>("Overview");
            data["{itemGenres}"] = _item.GetGenres();
            data["{itemDuration}"] = _item.GetDuration();
            data["{seriesTitle}"] = _item.GetSeriesTitle();
            data["{seasonNumber}"] = _item.GetSeasonNumber();
            data["{episodeNumber}"] = _item.GetEpisodeNumber();

            return data;
        }

        public override string? GetImagePath()
        {
            return _item.GetImagePath();
        }
    }
}
