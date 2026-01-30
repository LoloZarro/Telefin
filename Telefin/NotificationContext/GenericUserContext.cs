using System;
using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Data.Events;
using Jellyfin.Database.Implementations.Entities;

namespace Telefin.NotificationContext
{
    internal sealed class GenericUserContext : NotificationContextBase
    {
        private readonly User? _user;

        public GenericUserContext(GenericEventArgs<User> eventArgs)
        {
            _user = eventArgs?.Argument;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            // Identity
            data["{userId}"] = _user?.Id.ToString();
            data["{username}"] = _user?.Username;
            data["{internalId}"] = _user?.InternalId.ToString(CultureInfo.InvariantCulture);

            // Authentication / login
            data["{authProvider}"] = _user?.AuthenticationProviderId;
            data["{passwordResetProvider}"] = _user?.PasswordResetProviderId;
            data["{invalidLoginAttempts}"] = _user?.InvalidLoginAttemptCount.ToString(CultureInfo.InvariantCulture);
            data["{lastLogin}"] = FormatDate(_user?.LastLoginDate);
            data["{lastActivity}"] = FormatDate(_user?.LastActivityDate);
            data["{loginAttemptsBeforeLockout}"] = _user?.LoginAttemptsBeforeLockout?.ToString(CultureInfo.InvariantCulture);

            // Password & access
            data["{mustUpdatePassword}"] = _user?.MustUpdatePassword.ToString();
            data["{hasPassword}"] = (_user?.Password != null).ToString();
            data["{enableLocalPassword}"] = _user?.EnableLocalPassword.ToString();
            data["{enableAutoLogin}"] = _user?.EnableAutoLogin.ToString();
            data["{enableUserPreferenceAccess}"] = _user?.EnableUserPreferenceAccess.ToString();

            // Profile
            data["{profileImage}"] = _user?.ProfileImage?.Path;     // adjust depending on ImageInfo
            data["{castReceiverId}"] = _user?.CastReceiverId;

            // Preferences: Audio & Subtitle
            data["{audioLang}"] = _user?.AudioLanguagePreference;
            data["{subtitleLang}"] = _user?.SubtitleLanguagePreference;
            data["{subtitleMode}"] = _user?.SubtitleMode.ToString();
            data["{playDefaultAudio}"] = _user?.PlayDefaultAudioTrack.ToString();
            data["{rememberAudioSelections}"] = _user?.RememberAudioSelections.ToString();
            data["{rememberSubtitleSelections}"] = _user?.RememberSubtitleSelections.ToString();

            // User UI preferences
            data["{displayMissingEpisodes}"] = _user?.DisplayMissingEpisodes.ToString();
            data["{displayCollectionsView}"] = _user?.DisplayCollectionsView.ToString();
            data["{hidePlayedInLatest}"] = _user?.HidePlayedInLatest.ToString();

            // Parental control
            data["{maxParentalRatingScore}"] = _user?.MaxParentalRatingScore?.ToString(CultureInfo.InvariantCulture);
            data["{maxParentalRatingSubScore}"] = _user?.MaxParentalRatingSubScore?.ToString(CultureInfo.InvariantCulture);

            // Bitrate limit
            data["{remoteClientBitrateLimit}"] = _user?.RemoteClientBitrateLimit?.ToString(CultureInfo.InvariantCulture);

            // Sessions & SyncPlay
            data["{maxActiveSessions}"] = _user?.MaxActiveSessions.ToString(CultureInfo.InvariantCulture);
            data["{syncPlayAccess}"] = _user?.SyncPlayAccess.ToString();

            return data;
        }

        private static string? FormatDate(DateTime? date)
        {
            return date?.ToString(CultureInfo.InvariantCulture);
        }
    }
}
