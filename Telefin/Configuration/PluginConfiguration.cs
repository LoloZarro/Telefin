using System;
using MediaBrowser.Model.Plugins;

namespace Telefin.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public PluginConfiguration()
    {
        ServerUrl = "http://localhost:8096";
        EnablePlugin = true;
        UserConfigurations = Array.Empty<UserConfiguration>();
    }

    public string ServerUrl { get; set; }

    public bool EnablePlugin { get; set; }

    public UserConfiguration[] UserConfigurations { get; set; }
}