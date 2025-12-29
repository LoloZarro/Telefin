namespace Telefin.Configuration;

public class UserConfiguration
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string BotToken { get; set; } = string.Empty;
    public string ChatIds { get; set; } = string.Empty;
    public string ThreadId { get; set; } = string.Empty;

    public bool EnableUser { get; set; }
    public bool SilentNotification { get; set; }
    public bool DoNotMentionOwnActivities { get; set; }
    public bool KeepSerieImage { get; set; }

    public bool ItemAdded { get; set; }

    public bool ItemAddedMovies { get; set; } = true;
    public string ItemAddedMoviesStringMessage { get; set; } = DefaultMessages.ItemAddedMovies;

    public bool ItemAddedSeries { get; set; } = true;
    public string ItemAddedSeriesStringMessage { get; set; } = DefaultMessages.ItemAddedSeries;

    public bool ItemAddedSeasons { get; set; } = true;
    public string ItemAddedSeasonsStringMessage { get; set; } = DefaultMessages.ItemAddedSeasons;

    public bool ItemAddedEpisodes { get; set; } = true;
    public string ItemAddedEpisodesStringMessage { get; set; } = DefaultMessages.ItemAddedEpisodes;

    public bool ItemAddedAlbums { get; set; } = true;
    public string ItemAddedAlbumsStringMessage { get; set; } = DefaultMessages.ItemAddedAlbums;

    public bool ItemAddedSongs { get; set; } = true;
    public string ItemAddedSongsStringMessage { get; set; } = DefaultMessages.ItemAddedSongs;

    public bool ItemAddedBooks { get; set; } = true;
    public string ItemAddedBooksStringMessage { get; set; } = DefaultMessages.ItemAddedBooks;

    public bool ItemDeleted { get; set; }

    public bool ItemDeletedMovies { get; set; } = true;
    public string ItemDeletedMoviesStringMessage { get; set; } = DefaultMessages.ItemDeletedMovies;

    public bool ItemDeletedSeries { get; set; } = true;
    public string ItemDeletedSeriesStringMessage { get; set; } = DefaultMessages.ItemDeletedSeries;

    public bool ItemDeletedSeasons { get; set; } = true;
    public string ItemDeletedSeasonsStringMessage { get; set; } = DefaultMessages.ItemDeletedSeasons;

    public bool ItemDeletedEpisodes { get; set; } = true;
    public string ItemDeletedEpisodesStringMessage { get; set; } = DefaultMessages.ItemDeletedEpisodes;

    public bool ItemDeletedAlbums { get; set; } = true;
    public string ItemDeletedAlbumsStringMessage { get; set; } = DefaultMessages.ItemDeletedAlbums;

    public bool ItemDeletedSongs { get; set; } = true;
    public string ItemDeletedSongsStringMessage { get; set; } = DefaultMessages.ItemDeletedSongs;

    public bool ItemDeletedBooks { get; set; } = true;
    public string ItemDeletedBooksStringMessage { get; set; } = DefaultMessages.ItemDeletedBooks;

    public bool PlaybackStart { get; set; }

    public bool PlaybackStartMovies { get; set; } = true;
    public string PlaybackStartMoviesStringMessage { get; set; } = DefaultMessages.PlaybackStartMovies;

    public bool PlaybackStartEpisodes { get; set; } = true;
    public string PlaybackStartEpisodesStringMessage { get; set; } = DefaultMessages.PlaybackStartEpisodes;

    public bool PlaybackProgress { get; set; }

    public bool PlaybackProgressMovies { get; set; } = true;
    public string PlaybackProgressMoviesStringMessage { get; set; } = DefaultMessages.PlaybackProgressMovies;

    public bool PlaybackProgressEpisodes { get; set; } = true;
    public string PlaybackProgressEpisodesStringMessage { get; set; } = DefaultMessages.PlaybackProgressEpisodes;

    public bool PlaybackStop { get; set; }

    public bool PlaybackStopMovies { get; set; } = true;
    public string PlaybackStopMoviesStringMessage { get; set; } = DefaultMessages.PlaybackStopMovies;

    public bool PlaybackStopEpisodes { get; set; } = true;
    public string PlaybackStopEpisodesStringMessage { get; set; } = DefaultMessages.PlaybackStopEpisodes;

    public bool SubtitleDownloadFailure { get; set; }
    public string SubtitleDownloadFailureStringMessage { get; set; } = DefaultMessages.SubtitleDownloadFailure;

    public bool AuthenticationFailure { get; set; }
    public string AuthenticationFailureStringMessage { get; set; } = DefaultMessages.AuthenticationFailure;

    public bool AuthenticationSuccess { get; set; }
    public string AuthenticationSuccessStringMessage { get; set; } = DefaultMessages.AuthenticationSuccess;

    public bool SessionStart { get; set; }
    public string SessionStartStringMessage { get; set; } = DefaultMessages.SessionStart;

    public bool PendingRestart { get; set; }
    public string PendingRestartStringMessage { get; set; } = DefaultMessages.PendingRestart;

    public bool TaskCompleted { get; set; }
    public string TaskCompletedStringMessage { get; set; } = DefaultMessages.TaskCompleted;

    public bool PluginInstallationCancelled { get; set; }
    public string PluginInstallationCancelledStringMessage { get; set; } = DefaultMessages.PluginInstallationCancelled;

    public bool PluginInstallationFailed { get; set; }
    public string PluginInstallationFailedStringMessage { get; set; } = DefaultMessages.PluginInstallationFailed;

    public bool PluginInstalled { get; set; }
    public string PluginInstalledStringMessage { get; set; } = DefaultMessages.PluginInstalled;

    public bool PluginInstalling { get; set; }
    public string PluginInstallingStringMessage { get; set; } = DefaultMessages.PluginInstalling;

    public bool PluginUninstalled { get; set; }
    public string PluginUninstalledStringMessage { get; set; } = DefaultMessages.PluginUninstalled;

    public bool PluginUpdated { get; set; }
    public string PluginUpdatedStringMessage { get; set; } = DefaultMessages.PluginUpdated;

    public bool UserCreated { get; set; }
    public string UserCreatedStringMessage { get; set; } = DefaultMessages.UserCreated;

    public bool UserDeleted { get; set; }
    public string UserDeletedStringMessage { get; set; } = DefaultMessages.UserDeleted;

    public bool UserLockedOut { get; set; }
    public string UserLockedOutStringMessage { get; set; } = DefaultMessages.UserLockedOut;

    public bool UserPasswordChanged { get; set; }
    public string UserPasswordChangedStringMessage { get; set; } = DefaultMessages.UserPasswordChanged;

    public bool UserUpdated { get; set; }
    public string UserUpdatedStringMessage { get; set; } = DefaultMessages.UserUpdated;

    public bool UserDataSaved { get; set; }
    public string UserDataSavedStringMessage { get; set; } = DefaultMessages.UserDataSaved;
}