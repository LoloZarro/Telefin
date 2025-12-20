using System.Linq;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using Telefin.Common.Enums;

namespace Telefin.Common.Extensions
{
    internal static class NotificationTypeExtensions
    {
        public static string? ToNotificationSubType(this NotificationType type, BaseItem item)
        {
            string subType;
            switch (item)
            {
                case Movie:
                    subType = $"{type}Movies";
                    break;

                case Series:
                    subType = $"{type}Series";
                    break;

                case Season:
                    subType = $"{type}Seasons";
                    break;

                case Episode:
                    subType = $"{type}Episodes";
                    break;

                case MusicAlbum:
                    subType = $"{type}Albums";
                    break;

                case Audio:
                    subType = $"{type}Songs";
                    break;

                case Book:
                    subType = $"{type}Books";
                    break;

                default:
                    return null;
            }

            // Check if the notification sub type actually exists
            var result = Plugin.Config?.UserConfigurations?.First()?.GetPropertySafely<bool?>(subType);

            return (result != null) ? subType : null;
        }
    }
}
