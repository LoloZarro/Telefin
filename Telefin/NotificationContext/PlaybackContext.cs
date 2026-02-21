using System.Collections.Generic;
using System.Globalization;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;

namespace Telefin.NotificationContext
{
    internal sealed class PlaybackContext : NotificationContextBase
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
            data["{itemOverview}"] = _item.GetPropertySafely<string?>("Overview")?.Sanitize();
            data["{itemGenres}"] = _item.GetGenres();
            data["{itemDuration}"] = _item.GetDuration();
            data["{seriesTitle}"] = _item.GetPropertySafely<string?>("SeriesName");
            data["{seasonNumber}"] = _item.GetSeasonNumber();
            data["{episodeNumber}"] = _item.GetEpisodeNumber();
            data["{imdbLink}"] = _item.GetProviderLink(RatingProvider.Imdb);
            data["{tvdbLink}"] = _item.GetProviderLink(RatingProvider.Tvdb);
            data["{tmdbLink}"] = _item.GetProviderLink(RatingProvider.Tmdb);
            data["{communityRating}"] = _item.GetCommunityRating()?.ToString(CultureInfo.InvariantCulture);
            data["{criticRating}"] = _item.GetCriticRating()?.ToString(CultureInfo.InvariantCulture);
            data["{ageRating}"] = _item.GetAgeRating();
            data["{studios}"] = _item.GetStudios();

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
