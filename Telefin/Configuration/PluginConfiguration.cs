using System;
using MediaBrowser.Model.Plugins;

namespace Telefin.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    private int _playbackStartDebounceMs;

    public PluginConfiguration()
    {
        _playbackStartDebounceMs = 0;

        ServerUrl = "http://localhost:8096";
        EnablePlugin = true;
        UserConfigurations = Array.Empty<UserConfiguration>();
    }

    public string ServerUrl { get; set; }

    public bool EnablePlugin { get; set; }

    public int PlaybackStartDebounceMs
    {
        get => _playbackStartDebounceMs;
        set => _playbackStartDebounceMs = Math.Clamp(value, 0, 60000);
    }

    public UserConfiguration[] UserConfigurations { get; set; }
}