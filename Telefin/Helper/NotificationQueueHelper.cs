using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Controller.Entities;
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
            var kind = item switch
            {
                Movie => MediaSubType.Movie,
                Series => MediaSubType.Series,
                Season => MediaSubType.Season,
                Episode => MediaSubType.Episode,
                _ => MediaSubType.Unknown
            };

            return new QueuedItemContainer(item.Id, kind);
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

            var movies = items.Where(x => x.MediaType == MediaSubType.Movie).ToArray();
            var series = items.Where(x => x.MediaType == MediaSubType.Series).ToArray();
            var seasons = items.Where(x => x.MediaType == MediaSubType.Season).ToArray();
            var episodes = items.Where(x => x.MediaType == MediaSubType.Episode).ToArray();

            var otherMediaItems = items
                .Where(x =>
                    x.MediaType != MediaSubType.Movie &&
                    x.MediaType != MediaSubType.Series &&
                    x.MediaType != MediaSubType.Season &&
                    x.MediaType != MediaSubType.Episode)
                .ToArray();

            // Fast lookup, avoid constant iteration later on
            var seriesById = series.ToDictionary(x => x.ItemId);
            var seasonById = seasons.ToDictionary(x => x.ItemId);


            // --- Group episodes by season ---
            var episodesBySeasonId = episodes
                .Select(e => new { Container = e, (e.BaseItem as Episode)?.SeasonId }) // Requires Episode.BaseItem to be loaded from library
                .Where(x => x.SeasonId.HasValue)
                .GroupBy(x => x.SeasonId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Container).ToArray());

            // --- Ensure season containers ---
            var createdSeasonCandidates = new List<QueuedItemContainer>();
            foreach (var (seasonId, eps) in episodesBySeasonId)
            {
                // Only create if multiple episodes present and no season container exists yet
                if (!seasonById.TryGetValue(seasonId, out var seasonContainer))
                {
                    if (eps.Length <= 1) { continue; } // Single episode, don't bundle

                    seasonContainer = new QueuedItemContainer(seasonId, MediaSubType.Season);
                    seasonById[seasonId] = seasonContainer; // add to fast lookup
                    createdSeasonCandidates.Add(seasonContainer);
                }

                seasonContainer.AddChildren(eps.Select(e => e.ItemId));
            }

            // --- Group seasons by series ---
            var allSeasons = seasonById.Values.ToArray();

            var seasonsBySeriesId = allSeasons
                .Select(s => new { Container = s, (s.BaseItem as Season)?.SeriesId }) // Requires Season.BaseItem to be loaded from library
                .Where(x => x.SeriesId.HasValue)
                .GroupBy(x => x.SeriesId!.Value)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Container).ToArray());

            // ---- Ensure series containers exist ---
            var createdSeriesCandidates = new List<QueuedItemContainer>();

            foreach (var (seriesId, seas) in seasonsBySeriesId)
            {
                // Only create if multiple seasons present and no series container exists yet
                if (!seriesById.TryGetValue(seriesId, out var seriesContainer))
                {
                    if (seas.Length <= 1) { continue; }

                    seriesContainer = new QueuedItemContainer(seriesId, MediaSubType.Series);
                    seriesById[seriesId] = seriesContainer;
                    createdSeriesCandidates.Add(seriesContainer);
                }

                seriesContainer.AddChildren(seas.Select(s => s.ItemId));

                // We also have to attach episodes (from each season's child list if present, else from grouping)
                foreach (var seasonContainer in seas)
                {
                    // Prefer already-attached season children (covers existing seasons too)
                    if (seasonContainer.ChildItemIds.Count > 0)
                    {
                        seriesContainer.AddChildren(seasonContainer.ChildItemIds);
                        continue;
                    }

                    // Fallback: attach from episode grouping if season didn't have children yet
                    if (episodesBySeasonId.TryGetValue(seasonContainer.ItemId, out var epsForSeason))
                    {
                        seriesContainer.AddChildren(epsForSeason.Select(e => e.ItemId));
                    }
                }
            }

            // ---------------------------
            // Notification selection logic
            // ---------------------------

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
                .Concat(createdSeasonCandidates)
                .Concat(createdSeriesCandidates)
                .Concat(series)                 // notify existing series containers too
                .Concat(seasonCandidates)       // notify seasons not covered by any series candidate
                .Concat(episodeCandidates)      // notify episodes not covered by any season candidate
                .Concat(otherMediaItems)        // always notify for other media types (Audio, Book, etc.)
                .ToList();

            return candidates;
        }
    }
}
