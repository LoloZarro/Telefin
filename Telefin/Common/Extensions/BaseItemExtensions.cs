using System;
using System.Globalization;
using System.Linq;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using Telefin.Helper;

namespace Telefin.Common.Extensions
{
    internal static class BaseItemExtensions
    {
        public static string? GetImagePath(this BaseItem item)
        {
            if (item is Episode episode)
            {
                return BuildImageUrl(episode?.SeriesId);
            }

            if (item.PrimaryImagePath is not null)
            {
                return BuildImageUrl(item?.Id);
            }

            return string.Empty;
        }

        public static string? GetSeriesTitle(this BaseItem? item)
        {
            if (item == null)
            {
                return null;
            }

            // For episodes/seasons: item.Series.Name
            var series = item.GetPropertySafely<string?>("Series");
            if (series != null)
            {
                return series.GetPropertySafely<string?>("Name");
            }

            // For series items themselves: use the item name as series title
            // (you can add additional checks on type name if you want)
            return item.GetPropertySafely<string>("Name");
        }

        public static string? GetSeasonNumber(this BaseItem? item)
        {
            if (item == null)
            {
                return null;
            }

            // For episodes: episode.Season.IndexNumber
            var seasonObj = item.GetPropertySafely<Season>("Season");
            if (seasonObj != null)
            {
                var seasonIndex = seasonObj.GetPropertySafely<string>("IndexNumber");
                if (seasonIndex != null && int.TryParse(seasonIndex.ToString(), out var sn))
                {
                    return sn.ToString("00", CultureInfo.InvariantCulture);
                }
            }

            // For season items: season.IndexNumber
            var index = item.GetPropertySafely<string>("IndexNumber");
            if (index != null && int.TryParse(index.ToString(), out var seasonNumber))
            {
                return seasonNumber.ToString("00", CultureInfo.InvariantCulture);
            }

            return null;
        }

        public static string? GetEpisodeNumber(this BaseItem? item)
        {
            if (item == null)
            {
                return null;
            }

            // For episodes: episode.IndexNumber
            var index = item.GetPropertySafely<string>("IndexNumber");
            if (index != null && int.TryParse(index.ToString(), out var episodeNumber))
            {
                return episodeNumber.ToString("00", CultureInfo.InvariantCulture);
            }

            return null;
        }

        public static string? GetGenres(this BaseItem? item)
        {
            if (item == null)
            {
                return null;
            }

            var genres = item.GetPropertySafely<string[]?>("Genres");

            if (genres == null)
            {
                return null;
            }

            var list = genres.ToList();
            return list.Count == 0 ? null : string.Join(", ", list);
        }

        public static string? GetDuration(this BaseItem? item)
        {
            if (item == null)
            {
                return null;
            }

            var runTimeTicks = item.GetPropertySafely<long?>("RunTimeTicks");

            if (!runTimeTicks.HasValue || runTimeTicks <= 0)
            {
                return null;
            }

            // ticks → TimeSpan. Jellyfin uses 10,000 ticks per millisecond (i.e. TimeSpan).
            var ts = TimeSpan.FromTicks(runTimeTicks.Value);

            if (ts.TotalMinutes < 1)
            {
                return $"{(int)ts.TotalSeconds} seconds";
            }

            if (ts.TotalHours < 1)
            {
                return $"{(int)ts.TotalMinutes} minutes";
            }

            return $"{(int)ts.TotalHours}h {ts.Minutes}m";
        }

        private static string? BuildImageUrl(Guid? itemId)
        {
            if (itemId is null)
            {
                return null;
            }

            var url = ConfigurationHelper.GetServerUrl();
            return new Uri(url, $"/Items/{itemId}/Images/Primary").ToString();
        }
    }
}
