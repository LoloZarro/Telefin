using System;

namespace Telefin.Helper
{
    internal static class ConfigurationHelper
    {
        public static Uri GetServerUrl()
        {
            var raw = Plugin.Instance?.Configuration.ServerUrl ?? "localhost:8096";

            var hasScheme = raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                         || raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

            var baseUri = new Uri(hasScheme ? raw : $"http://{raw}", UriKind.Absolute); // Do not remove scheme if present, could lead to issues

            return baseUri;
        }

        public static int PlaybackStartDebounceMs()
        {
            return Plugin.Instance?.Configuration.PlaybackStartDebounceMs ?? 0;
        }
    }
}
