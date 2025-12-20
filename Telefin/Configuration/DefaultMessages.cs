namespace Telefin.Configuration
{
    public static class DefaultMessages
    {
        public static string ItemAddedMovies { get; } =
            "🎬 {item.Name} ({item.ProductionYear})\n" +
            "      added to library\n\n" +
            "📽 {item.Overview}";

        public static string ItemAddedSeries { get; } =
            "📺 [Serie] {serie.Name} ({item.ProductionYear}) added to library\n\n" +
            "📽 {item.Overview}";

        public static string ItemAddedSeasons { get; } =
            "📺 {season.Series.Name} ({item.ProductionYear})\n" +
            "      Season {seasonNumber} added to library\n\n" +
            "📽 {item.Overview}";

        public static string ItemAddedEpisodes { get; } =
            "📺 {episode.Series.Name} ({item.ProductionYear})\n" +
            "      S{eSeasonNumber} - E{episodeNumber}\n" +
            "      '{item.Name}' added to library\n\n" +
            "📽 {item.Overview}";

        public static string ItemAddedAlbums { get; } =
            "🎵 [Album] {album.Name} ({item.ProductionYear}) added to library";

        public static string ItemAddedSongs { get; } =
            "🎵 [Audio] {audio.Name} ({item.ProductionYear}) added to library";

        public static string ItemAddedBooks { get; } =
            "📖 [Book] {item.Name} added to library\n\n" +
            "🖋️ {item.Overview}";

        public static string ItemDeletedMovies { get; } =
            "🗑️🎬 {item.Name} ({item.ProductionYear})\n" +
            "      removed from library\n\n" +
            "📽 {item.Overview}";

        public static string ItemDeletedSeries { get; } =
            "🗑️📺 [Serie] {serie.Name} ({item.ProductionYear}) removed from library\n\n" +
            "📽 {item.Overview}";

        public static string ItemDeletedSeasons { get; } =
            "🗑️📺 {season.Series.Name} ({item.ProductionYear})\n" +
            "      Season {seasonNumber} removed from library\n\n" +
            "📽 {item.Overview}";

        public static string ItemDeletedEpisodes { get; } =
            "🗑️📺 {episode.Series.Name} ({item.ProductionYear})\n" +
            "      S{eSeasonNumber} - E{episodeNumber}\n" +
            "      '{item.Name}' removed from library\n\n" +
            "📽 {item.Overview}";

        public static string ItemDeletedAlbums { get; } =
            "🗑️🎵 [Album] {album.Name} ({item.ProductionYear}) removed from library";

        public static string ItemDeletedSongs { get; } =
            "🗑️🎵 [Audio] {audio.Name} ({item.ProductionYear}) removed from library";

        public static string ItemDeletedBooks { get; } =
            "🗑️📖 [Book] {item.Name} removed from library\n\n" +
            "🖋️ {item.Overview}";

        public static string AuthenticationFailure { get; } =
            "🔒 Authentication failure on {eventArgs.Argument.DeviceName} for user {eventArgs.Argument.Username}";

        public static string AuthenticationSuccess { get; } =
            "🔓 Authentication success for user {eventArgs.Argument.User.Name} on {eventArgs.Argument.SessionInfo.DeviceName}";

        public static string PendingRestart { get; } =
            "🔄 Jellyfin is pending a restart.";

        public static string PlaybackProgressMovies { get; } =
            "👤 {eventArgs.Users[0].Username} is still watching on {eventArgs.DeviceName}:\n" +
            "🎬 {eventArgs.Item.Name} ({eventArgs.Item.ProductionYear})";

        public static string PlaybackProgressEpisodes { get; } =
            "👤 {eventArgs.Users[0].Username} is still watching on {eventArgs.DeviceName}:\n" +
            "🎬 {eventArgs.Item.Series.Name} ({eventArgs.Item.ProductionYear})\n" +
            "      S{playbackSeasonNumber} - E{playbackEpisodeNumber}\n" +
            "      '{eventArgs.Item.Name}'";

        public static string PlaybackStartMovies { get; } =
            "👤 {username} is watching on {deviceName} ({playMethod}):\n" +
            "🎬 {itemName} ({itemYear})\n" +
            "📺 [{itemMediaType}] {itemGenres}\n" +
            "🕒 {duration}\n" +
            "📽 {overview}";

        public static string PlaybackStartEpisodes { get; } =
            "👤 {eventArgs.Users[0].Username} is watching on {eventArgs.DeviceName} ({eventArgs.Session.PlayState.PlayMethod}):\n" +
            "🎬 {eventArgs.Item.Series.Name} ({eventArgs.Item.ProductionYear})\n" +
            "      S{playbackSeasonNumber} - E{playbackEpisodeNumber}\n" +
            "      '{eventArgs.Item.Name}'\n" +
            "📺 [{eventArgs.Item.MediaType}] {eventArgs.Item.Series.Genres}\n" +
            "🕒 {duration}\n" +
            "📽 {eventArgs.Item.Overview}";

        public static string PlaybackStopMovies { get; } =
            "👤 {eventArgs.Users[0].Username} stopped watching:\n" +
            "🎬 {eventArgs.Item.Name} ({eventArgs.Item.ProductionYear})";

        public static string PlaybackStopEpisodes { get; } =
            "👤 {eventArgs.Users[0].Username} stopped watching:\n" +
            "🎬 {eventArgs.Item.Series.Name} ({eventArgs.Item.ProductionYear})\n" +
            "      S{playbackSeasonNumber} - E{playbackEpisodeNumber}\n" +
            "      '{eventArgs.Item.Name}'";

        public static string PluginInstallationCancelled { get; } =
            "🔴 {eventArgs.Argument.Name} plugin installation cancelled (version {eventArgs.Argument.Version}):";

        public static string PluginInstallationFailed { get; } =
            "🔴 {eventArgs.InstallationInfo} plugin installation failed (version {eventArgs.VersionInfo}):\n" +
            "{eventArgs.Exception}";

        public static string PluginInstalled { get; } =
            "🚧 {eventArgs.Argument.Name} plugin installed (version {eventArgs.Argument.Version})\n\n" +
            "You may need to restart your server.";

        public static string PluginInstalling { get; } =
            "🚧 {eventArgs.Argument.Name} plugin is installing (version {eventArgs.Argument.Version})";

        public static string PluginUninstalled { get; } =
            "🚧 {eventArgs.Argument.Name} plugin uninstalled";

        public static string PluginUpdated { get; } =
            "🚧 {eventArgs.Argument.Name} plugin updated to version {eventArgs.Argument.Version}:" +
            "🗒️ {eventArgs.Argument.Changelog}\n\n" +
            "You may need to restart Jellyfin to apply the changes.";

        public static string SessionStart { get; } =
            "👤 {eventArgs.Argument.UserName} has started a session on:\n" +
            "💻 {eventArgs.Argument.Client} ({eventArgs.Argument.DeviceName})\n";

        public static string SubtitleDownloadFailure { get; } =
            "🚫 Subtitle download failed for {eventArgs.Item.Name}";

        public static string TaskCompleted { get; } =
            "🧰 Task {eventArgs.Task.Name} completed: {eventArgs.Task.CurrentProgress}%\n" +
            "🗒️ ({eventArgs.Task.Category}) {eventArgs.Task.Description}";

        public static string UserCreated { get; } =
            "👤 User {eventArgs.Argument.Username} created.";

        public static string UserDeleted { get; } =
            "🗑️ User {eventArgs.Argument.Username} deleted.";

        public static string UserLockedOut { get; } =
            "👤🔒 User {eventArgs.Argument.Username} locked out";

        public static string UserPasswordChanged { get; } =
            "👤 User {eventArgs.Argument.Username} changed his password.";

        public static string UserUpdated { get; } =
            "👤 User {eventArgs.Argument.Username} has been updated";

        public static string UserDataSaved { get; } =
            "👤 User {eventArgs.Argument.Username} data saved.";
    }

}
