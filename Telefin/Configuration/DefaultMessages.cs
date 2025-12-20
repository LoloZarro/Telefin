namespace Telefin.Configuration
{
    public static class DefaultMessages
    {
        // LIBRARY: ITEM ADDED

        public static string ItemAddedMovies { get; } =
            "🎬 {itemTitle} ({itemYear})\n" +
            "      has been added to your library\n\n" +
            "📺 [{mediaType}] {itemGenres}\n" +
            "🕒 {itemDuration}\n" +
            "📽 {itemOverview}";

        public static string ItemAddedSeries { get; } =
            "📺 {itemTitle} ({itemYear})\n" +
            "      series added to your library\n\n" +
            "📺 Genres: {itemGenres}\n" +
            "📽 {itemOverview}";

        public static string ItemAddedSeasons { get; } =
            "📺 {seriesTitle} ({itemYear})\n" +
            "      Season {seasonNumber} added to your library\n\n" +
            "📽 {itemOverview}";

        public static string ItemAddedEpisodes { get; } =
            "📺 {seriesTitle} ({itemYear})\n" +
            "      S{seasonNumber} · E{episodeNumber}: '{itemTitle}' added to your library\n\n" +
            "📽 {itemOverview}";

        public static string ItemAddedAlbums { get; } =
            "🎵 Album {itemTitle} ({itemYear}) added to your library";

        public static string ItemAddedSongs { get; } =
            "🎵 Track {itemTitle} ({itemYear}) added to your library";

        public static string ItemAddedBooks { get; } =
            "📖 {itemTitle} added to your library\n\n" +
            "🖋️ {itemOverview}";

        // LIBRARY: ITEM DELETED

        public static string ItemDeletedMovies { get; } =
            "🗑️🎬 {itemTitle} ({itemYear})\n" +
            "      removed from your library\n\n" +
            "📽 {itemOverview}";

        public static string ItemDeletedSeries { get; } =
            "🗑️📺 {itemTitle} ({itemYear}) series removed from your library\n\n" +
            "📽 {itemOverview}";

        public static string ItemDeletedSeasons { get; } =
            "🗑️📺 {seriesTitle} ({itemYear})\n" +
            "      Season {seasonNumber} removed from your library\n\n" +
            "📽 {itemOverview}";

        public static string ItemDeletedEpisodes { get; } =
            "🗑️📺 {seriesTitle} ({itemYear})\n" +
            "      S{seasonNumber} · E{episodeNumber}: '{itemTitle}' removed from your library\n\n" +
            "📽 {itemOverview}";

        public static string ItemDeletedAlbums { get; } =
            "🗑️🎵 Album {itemTitle} ({itemYear}) removed from your library";

        public static string ItemDeletedSongs { get; } =
            "🗑️🎵 Track {itemTitle} ({itemYear}) removed from your library";

        public static string ItemDeletedBooks { get; } =
            "🗑️📖 {itemTitle} removed from your library\n\n" +
            "🖋️ {itemOverview}";

        // AUTHENTICATION

        public static string AuthenticationFailure { get; } =
            "🔒 Authentication failed on {deviceName} for user {username}";

        public static string AuthenticationSuccess { get; } =
            "🔓 User {username} successfully signed in on {deviceName} (server: {serverName}, last activity: {lastActivity})";

        // SERVER

        public static string PendingRestart { get; } =
            "🔄 Jellyfin is pending a restart.";

        // PLAYBACK: PROGRESS

        public static string PlaybackProgressMovies { get; } =
            "👤 {username} is still watching on {deviceName}:\n" +
            "🎬 {itemTitle} ({itemYear})\n" +
            "🕒 {itemDuration}";

        public static string PlaybackProgressEpisodes { get; } =
            "👤 {username} is still watching on {deviceName}:\n" +
            "📺 {seriesTitle} ({itemYear})\n" +
            "      S{seasonNumber} · E{episodeNumber}: '{itemTitle}'\n" +
            "🕒 {itemDuration}";

        // PLAYBACK: START

        public static string PlaybackStartMovies { get; } =
            "👤 {username} is watching on {deviceName} ({playMethod}):\n" +
            "🎬 {itemTitle} ({itemYear})\n" +
            "📺 [{mediaType}] {itemGenres}\n" +
            "🕒 {itemDuration}\n" +
            "📽 {itemOverview}";

        public static string PlaybackStartEpisodes { get; } =
            "👤 {username} is watching on {deviceName} ({playMethod}):\n" +
            "📺 {seriesTitle} ({itemYear})\n" +
            "      S{seasonNumber} · E{episodeNumber}: '{itemTitle}'\n" +
            "📺 [{mediaType}] {itemGenres}\n" +
            "🕒 {itemDuration}\n" +
            "📽 {itemOverview}";

        // PLAYBACK: STOP

        public static string PlaybackStopMovies { get; } =
            "👤 {username} stopped watching:\n" +
            "🎬 {itemTitle} ({itemYear})";

        public static string PlaybackStopEpisodes { get; } =
            "👤 {username} stopped watching:\n" +
            "📺 {seriesTitle} ({itemYear})\n" +
            "      S{seasonNumber} · E{episodeNumber}: '{itemTitle}'";

        // PLUGINS

        public static string PluginInstallationCancelled { get; } =
            "🔴 Plugin {itemName} installation cancelled (version {itemVersion}).";

        public static string PluginInstallationFailed { get; } =
            "🔴 Plugin {itemName} installation failed (version {itemVersion}).\n" +
            "🧩 Package: {itemPackageName}\n" +
            "🗒️ {itemPackageDescription}\n" +
            "⚠️ Error: {errorMessage}";

        public static string PluginInstalled { get; } =
            "✅ Plugin {itemName} installed (version {itemVersion}).\n" +
            "🧩 Package: {itemPackageName}\n\n" +
            "You may need to restart your server.";

        public static string PluginInstalling { get; } =
            "🚧 Plugin {itemName} is installing (version {itemVersion})…";

        public static string PluginUninstalled { get; } =
            "🚧 Plugin {itemName} (version {itemVersion}) uninstalled.\n" +
            "🗒️ {itemDescription}\n" +
            "Status: {itemStatus}";

        public static string PluginUpdated { get; } =
            "🚧 Plugin {itemName} updated to version {itemVersion}.\n" +
            "🧩 Package: {itemPackageName}\n" +
            "🗒️ {itemPackageOverview}\n\n" +
            "You may need to restart Jellyfin to apply the changes.";

        // SESSION (if wired to something that exposes {username}/{deviceName})

        public static string SessionStart { get; } =
            "👤 {username} has started a session on {deviceName}.";

        // SUBTITLES

        public static string SubtitleDownloadFailure { get; } =
            "🚫 Subtitle download failed from {subtitleProvider} for {itemTitle} ({itemYear}).\n" +
            "⚠️ {errorMessage}";

        // TASKS

        public static string TaskCompleted { get; } =
            "🧰 Task {taskName} completed with status {status}.\n" +
            "⏱️ Duration: {durationMinutes} minutes\n" +
            "🗒️ ({taskCategory}) {taskDescription}\n" +
            "⚠️ {error}";

        // USERS

        public static string UserCreated { get; } =
            "👤 User {username} created.";

        public static string UserDeleted { get; } =
            "🗑️ User {username} deleted.";

        public static string UserLockedOut { get; } =
            "👤🔒 User {username} locked out.";

        public static string UserPasswordChanged { get; } =
            "👤 User {username} changed their password.";

        public static string UserUpdated { get; } =
            "👤 User {username} has been updated.";

        public static string UserDataSaved { get; } =
            "👤 User {username} data saved.";
    }
}
