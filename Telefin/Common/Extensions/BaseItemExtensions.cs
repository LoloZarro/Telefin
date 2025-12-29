using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using Telefin.Common.Enums;
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
            if (item == null || item is Movie)
            {
                return null;
            }

            if (item is Series series)
            {
                return item.GetPropertySafely<string>("Name");
            }

            return item.GetPropertySafely<string?>("SeriesName");
        }

        public static string? GetSeasonNumber(this BaseItem? item)
        {
            if (item == null || item is not Season)
            {
                return null;
            }

            int seasonNumber;

            var season = item.GetPropertySafely<Season>("Season");
            if (season != null)
            {
                var seasonIndex = season.GetPropertySafely<string>("IndexNumber");
                if (seasonIndex != null && int.TryParse(seasonIndex.ToString(), out seasonNumber))
                {
                    return seasonNumber.ToString("00", CultureInfo.InvariantCulture);
                }
            }

            var index = item.GetPropertySafely<string>("IndexNumber");
            if (index != null && int.TryParse(index.ToString(), out seasonNumber))
            {
                return seasonNumber.ToString("00", CultureInfo.InvariantCulture);
            }

            return null;
        }

        public static string? GetEpisodeNumber(this BaseItem? item)
        {
            if (item == null || item is not Episode)
            {
                return null;
            }

            var index = item.GetPropertySafely<string>("IndexNumber");
            if (index != null && int.TryParse(index.ToString(), out var episodeNumber))
            {
                return episodeNumber.ToString("00", CultureInfo.InvariantCulture);
            }

            return null;
        }

        public static string? GetEpisodeAmount(this BaseItem? item)
        {
            if (item == null || item is not Series || item is not Season)
            {
                return null;
            }

            BaseItem[]? episodes = null;

            if (item is Series series)
            {
                var seasons = series.GetPropertySafely<List<BaseItem>?>("Children");
                episodes = seasons?.SelectMany(season => season.GetPropertySafely<BaseItem[]>("Children") ?? Array.Empty<BaseItem>())?.ToArray();
            }
            else
            {
                episodes = item.GetPropertySafely<BaseItem[]?>("Children");
            }

            return episodes?.Length.ToString(CultureInfo.InvariantCulture);
        }

        public static string? GetSeasonAmount(this BaseItem? item)
        {
            if (item == null || item is not Series)
            {
                return null;
            }

            if (item is Series series)
            {
                return series.GetPropertySafely<BaseItem[]?>("Children")?.Length.ToString(CultureInfo.InvariantCulture);
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

            if (genres == null || genres.Length <= 0)
            {
                genres = item.GetPropertySafely<BaseItem>("DisplayParent")?.GetPropertySafely<string[]?>("Genres");
            }

            if (genres == null || genres.Length <= 0)
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

            // ticks -> TimeSpan. Jellyfin uses 10,000 ticks per millisecond
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

        public static string? GetProviderLink(this BaseItem? item, RatingProvider provider)
        {
            if (item == null)
            {
                return null;
            }

            (BaseItem? source, bool isSeries) = item switch
            {
                Movie => (item, false),
                Series or Episode => (item, true),
                Season => (item.GetPropertySafely<BaseItem?>("DisplayParent"), true),
                _ => (null, false)
            };

            if (source == null)
            {
                return null;
            }

            var providers = source.GetPropertySafely<IDictionary<string, string>?>("ProviderIds");

            if (providers == null || providers.Count <= 0)
            {
                return null;
            }

            if (!providers.TryGetValue(provider.ToString(), out var id) || string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return provider switch
            {
                RatingProvider.Imdb => $"https://imdb.com/title/{id}",
                RatingProvider.Tvdb => $"https://www.thetvdb.com/dereferrer/{(isSeries ? ((item is Episode) ? "episode" : "series") : "movies")}/{id}",
                RatingProvider.Tmdb => $"https://www.themoviedb.org/{(isSeries ? "tv" : "movie")}/{id}",
                _ => null
            };
        }

        public static float? GetCriticRating(this BaseItem? item)
        {
            if (item == null)
            {
                return null;
            }

            if (item is Season)
            {
                return item.GetPropertySafely<Series?>("DisplayParent")?.GetPropertySafely<float?>("CriticRating");
            }

            return item.GetPropertySafely<float?>("CriticRating");
        }

        public static float? GetCommunityRating(this BaseItem? item)
        {
            if (item == null)
            {
                return null;
            }

            if (item is Season)
            {
                return item.GetPropertySafely<Series?>("DisplayParent")?.GetPropertySafely<float?>("CommunityRating");
            }

            return item.GetPropertySafely<float?>("CommunityRating");
        }

        public static string? GetAgeRating(this BaseItem? item)
        {
            if (item == null)
            {
                return null;
            }

            return item.GetPropertySafely<string?>("OfficialRatingForComparison");
        }

        public static string? GetStudios(this BaseItem? item)
        {
            if (item == null)
            {
                return null;
            }

            var source = item switch
            {
                Movie => item,
                Series => item,
                Season => item.GetPropertySafely<Series?>("DisplayParent"),
                Episode => item.GetPropertySafely<Series?>("DisplayParent")?.GetPropertySafely<Series?>("DisplayParent"),
                _ => null,
            };

            var studios = source?.GetPropertySafely<string[]?>("Studios")?.Where(s => !string.IsNullOrWhiteSpace(s));

            if (studios == null)
            {
                return null;
            }

            return string.Join(", ", studios);
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
