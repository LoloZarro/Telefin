using System.Collections.Generic;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using Telefin.Common.Extensions;

namespace Telefin.NotificationContext
{
    internal class PlaybackContext : NotificationContextBase
    {
        private readonly PlaybackProgressEventArgs _eventArgs;
        private readonly BaseItem _item;

        public PlaybackContext(PlaybackProgressEventArgs eventArgs)
        {
            _eventArgs = eventArgs;
            _item = eventArgs.Item;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{username}"] = SafeFirstUserName(_eventArgs);
            data["{deviceName}"] = _eventArgs.DeviceName;
            data["{playMethod}"] = _eventArgs.Session?.PlayState?.PlayMethod?.ToString();
            data["{itemTitle}"] = _item.GetPropertySafely<string?>("Name");
            data["{itemYear}"] = _item.GetPropertySafely<string?>("ProductionYear");
            data["{mediaType}"] = _item.GetPropertySafely<string?>("MediaType");
            data["{itemOverview}"] = _item.GetPropertySafely<string?>("Overview");
            data["{itemGenres}"] = _item.GetGenres();
            data["{itemDuration}"] = _item.GetDuration();
            data["{seriesTitle}"] = _item.GetPropertySafely<string?>("SeriesName");
            data["{seasonNumber}"] = _item.GetSeasonNumber();
            data["{episodeNumber}"] = _item.GetEpisodeNumber();

            return data;
        }

        public override string? GetImagePath()
        {
            return _item.GetImagePath();
        }

        private static string? SafeFirstUserName(dynamic args)
        {
            try
            {
                var users = args.Users as IEnumerable<object>;
                if (users == null)
                {
                    return null;
                }

                foreach (var u in users)
                {
                    var username = u.GetPropertySafely<string>("Username");
                    if (!string.IsNullOrWhiteSpace(username))
                    {
                        return username;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
