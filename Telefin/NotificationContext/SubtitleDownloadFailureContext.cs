using System.Collections.Generic;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Subtitles;
using Telefin.Common.Extensions;

namespace Telefin.NotificationContext
{
    internal sealed class SubtitleDownloadFailureContext : NotificationContextBase
    {
        private readonly SubtitleDownloadFailureEventArgs _eventArgs;
        private readonly BaseItem _item;

        public SubtitleDownloadFailureContext(SubtitleDownloadFailureEventArgs eventArgs)
        {
            _eventArgs = eventArgs;
            _item = eventArgs.Item;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{subtitleProvider}"] = _eventArgs.Provider;
            data["{errorMessage}"] = _eventArgs.Exception.Message;
            data["{itemTitle}"] = _item.GetPropertySafely<string?>("Name");
            data["{itemYear}"] = _item.GetPropertySafely<string?>("ProductionYear");
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
