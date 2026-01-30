using System.Collections.Generic;
using System.Globalization;
using MediaBrowser.Controller.Entities;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;

namespace Telefin.NotificationContext
{
    internal sealed class LibraryItemAddedContext : NotificationContextBase
    {
        private readonly BaseItem _item;

        public LibraryItemAddedContext(BaseItem item)
        {
            _item = item;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            data["{itemTitle}"] = _item.GetPropertySafely<string?>("Name");
            data["{itemYear}"] = _item.GetPropertySafely<string?>("ProductionYear");
            data["{itemOverview}"] = _item.GetPropertySafely<string?>("Overview");
            data["{itemGenres}"] = _item.GetGenres();
            data["{itemDuration}"] = _item.GetDuration();
            data["{seriesTitle}"] = _item.GetSeriesTitle();
            data["{seasonNumber}"] = _item.GetSeasonNumber();
            data["{episodeNumber}"] = _item.GetEpisodeNumber();
            data["{seasonAmount}"] = _item.GetSeasonAmount();
            data["{episodeAmount}"] = _item.GetEpisodeAmount();
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
    }
}
