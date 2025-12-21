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
    private readonly ILogger<Plugin> _logger;

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger)
            : base(applicationPaths, xmlSerializer)
    {
        _logger = logger;
        Instance = this;
    }

    public static ILogger<Plugin> Logger => Instance!._logger;

    public static PluginConfiguration Config => Instance!.Configuration;

    public override string Name => "Telefin";

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
}