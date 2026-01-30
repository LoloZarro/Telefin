using System;
using System.Collections.Generic;
using System.Globalization;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using Telefin.Configuration;

namespace Telefin;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public const string PluginName = "Telefin";

    private readonly ILogger<Plugin> _logger;

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger)
            : base(applicationPaths, xmlSerializer)
    {
        _logger = logger;
        Instance = this;

        EnsureConfigurationDefaults();
    }

    public static ILogger<Plugin> Logger => Instance!._logger;

    public static PluginConfiguration Config => Instance!.Configuration;

    public override string Name => PluginName;

    public override Guid Id => Guid.Parse("91ce115a-3f38-49bc-b36c-0f688b1495c7");

    public static Plugin? Instance { get; private set; }

    public IEnumerable<PluginPageInfo> GetPages()
    {
        yield return new PluginPageInfo
        {
            Name = Name,
            EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Web.configPage.html", GetType().Namespace)
        };
        yield return new PluginPageInfo
        {
            Name = $"{Name}.js",
            EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Web.configPage.js", GetType().Namespace)
        };
    }

    private void EnsureConfigurationDefaults()
    {
        var cfg = Configuration;
        var changed = false;

        if (cfg.PlaybackStartDebounceMs < 0 || cfg.PlaybackStartDebounceMs > 60000)
        {
            cfg.PlaybackStartDebounceMs = 0;
            changed = true;
        }

        if (changed)
        {
            SaveConfiguration();
        }
    }
}