using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Library;
using Telefin.Common.Enums;
using Telefin.Common.Models;

namespace Telefin.Helper
{
    internal static class NotificationQueueHelper
    {
        public static QueuedItemContainer CreateContainer(BaseItem item, ILibraryManager libraryManager)
        {
            var mediaSubType = item switch
            {
                Movie => MediaType.Movie,
                Series => MediaType.Series,
                Season => MediaType.Season,
                Episode => MediaType.Episode,
                Video => MediaType.Video,
                MusicAlbum => MediaType.MusicAlbum,
                AudioBook => MediaType.AudioBook,
                Audio => MediaType.Audio,
                Book => MediaType.Book,

                _ => MediaType.Unknown
            };

            return new QueuedItemContainer(item, mediaSubType);
        }

        /// <summary>
        /// Evaluates the current notification queue and determines the optimal set of
        /// items to notify about. The method analyzes queued items by media type and
        /// groups related TV content hierarchically (episodes → seasons → series) to
        /// prevent redundant notifications when multiple related items are added
        /// together.
        ///
        /// Episodes belonging to the same season may be bundled into a season
        /// notification, and multiple seasons may be promoted into a series
        /// notification when appropriate. Higher-level candidates are enriched with
        /// the IDs of their child items.
        ///
        /// The resulting list contains only notification candidates that are not
        /// already covered by a parent notification, ensuring concise and meaningful
        /// notifications while preserving standalone items such as movies.
        /// </summary>
        /// <param name="queue">
        /// A snapshot of queued items keyed by item ID, each containing metadata used
        /// to evaluate grouping and notification eligibility.
        /// </param>
        /// <returns>
        /// A list of finalized notification candidates ready for dispatch.
        /// </returns>
        public static List<QueuedItemContainer> EvaluateNotificationCandidates(KeyValuePair<Guid, QueuedItemContainer>[] queue)
        {
            var items = queue?.Select(x => x.Value).Where(x => x is not null).ToArray() ?? []; // We only need the containers from here on

            var movies = items.Where(x => x.MediaType == MediaType.Movie).ToArray();
            var series = items.Where(x => x.MediaType == MediaType.Series).ToArray();
            var seasons = items.Where(x => x.MediaType == MediaType.Season).ToArray();
            var episodes = items.Where(x => x.MediaType == MediaType.Episode).ToArray();

            var otherMediaItems = items
                .Where(x =>
                    x.MediaType != MediaType.Movie &&
                    x.MediaType != MediaType.Series &&
                    x.MediaType != MediaType.Season &&
                    x.MediaType != MediaType.Episode)
                .ToArray();

            var seriesById = series.ToDictionary(x => x.ItemId);
            var seasonById = seasons.ToDictionary(x => x.ItemId);

            // Seasons

            var episodesBySeasonId = episodes
                .Select(e => new { Container = e, (e.BaseItem as Episode)?.SeasonId }) // Requires Episode.BaseItem to be loaded from library
                .Where(x => x.SeasonId.HasValue)
                .GroupBy(x => x.SeasonId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Container).ToArray());

            var createdSeasonCandidates = new List<QueuedItemContainer>();
            foreach (var (seasonId, eps) in episodesBySeasonId)
            {
                // Only create if multiple episodes present and no season container exists yet
                if (!seasonById.TryGetValue(seasonId, out var seasonContainer))
                {
                    if (eps.Length <= 1)
                    {
                        continue;
                    }

                    seasonContainer = new QueuedItemContainer(seasonId, MediaType.Season);
                    seasonById[seasonId] = seasonContainer;
                    createdSeasonCandidates.Add(seasonContainer);
                }

                seasonContainer.RemoveAllChildren(); // Remove existing children just to be save (seasons will often already contain episodes that belong to it)
                seasonContainer.AddChildren(eps.Select(e => e.ItemId));
            }

            // Series

            var allSeasons = seasonById.Values.ToArray();

            var seasonsBySeriesId = allSeasons
                .Select(s => new { Container = s, (s.BaseItem as Season)?.SeriesId }) // Requires Season.BaseItem to be loaded from library
                .Where(x => x.SeriesId.HasValue)
                .GroupBy(x => x.SeriesId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Container).ToArray());

            var createdSeriesCandidates = new List<QueuedItemContainer>();

            foreach (var (seriesId, seas) in seasonsBySeriesId)
            {
                // Only create if multiple seasons present and no series container exists yet
                if (!seriesById.TryGetValue(seriesId, out var seriesContainer))
                {
                    if (seas.Length <= 1)
                    {
                        continue;
                    }

                    seriesContainer = new QueuedItemContainer(seriesId, MediaType.Series);
                    seriesById[seriesId] = seriesContainer;
                    createdSeriesCandidates.Add(seriesContainer);
                }

                seriesContainer.RemoveAllChildren(); // Remove existing children just to be save (series will often already contain seasons that belong to it)
                seriesContainer.AddChildren(seas.Select(s => s.ItemId));
                seriesContainer.AddChildren(seas.SelectMany(s => s.ChildItemIds));
            }

            // Evaluation of candidates

            var seasonIdsCoveredByAnySeries = series
                .SelectMany(s => s.ChildItemIds)
                .Concat(createdSeriesCandidates.SelectMany(s => s.ChildItemIds))
                .ToHashSet();

            var episodeIdsCoveredByAnySeries = series
                .SelectMany(s => s.ChildItemIds)
                .Concat(createdSeriesCandidates.SelectMany(s => s.ChildItemIds))
                .ToHashSet();

            var episodeIdsCoveredByAnySeason = seasons
                .SelectMany(s => s.ChildItemIds)
                .Concat(createdSeasonCandidates.SelectMany(s => s.ChildItemIds))
                .ToHashSet();

            var seasonCandidates = seasons
                .Where(season => !seasonIdsCoveredByAnySeries.Contains(season.ItemId));

            var episodeCandidates = episodes
                .Where(episode =>
                    !episodeIdsCoveredByAnySeries.Contains(episode.ItemId) &&
                    !episodeIdsCoveredByAnySeason.Contains(episode.ItemId));

            var candidates = movies
                .Concat(series)
                .Concat(createdSeriesCandidates)
                .Concat(seasonCandidates)
                .Concat(createdSeasonCandidates)
                .Concat(episodeCandidates)
                .Concat(otherMediaItems)
                .ToList();

            return candidates;
        }
    }
}
