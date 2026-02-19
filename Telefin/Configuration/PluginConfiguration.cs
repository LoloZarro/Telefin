using System;
using MediaBrowser.Model.Plugins;

namespace Telefin.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    private int _metadataWaitMultiplier;

    private int _playbackStartDebounceMs;

    public PluginConfiguration()
    {
        _metadataWaitMultiplier = 10;
        _playbackStartDebounceMs = 0;

        ServerUrl = "http://localhost:8096";
        EnablePlugin = true;
        SuppressMovedMediaNotifications = false;
        UserConfigurations = Array.Empty<UserConfiguration>();
    }

    public string ServerUrl { get; set; }

    public bool EnablePlugin { get; set; }

    public int MetadataWaitMultiplier
    {
        get => _metadataWaitMultiplier;
        set => _metadataWaitMultiplier = Math.Clamp(value, 1, 100);
    }

    public int PlaybackStartDebounceMs
    {
        get => _playbackStartDebounceMs;
        set => _playbackStartDebounceMs = Math.Clamp(value, 0, 60000);
    }

    public bool SuppressMovedMediaNotifications { get; set; }

    public UserConfiguration[] UserConfigurations { get; set; }
}