using System;

namespace Telefin.Helper
{
    internal static class ConfigurationHelper
    {
        public static bool GetEnablePlugin()
        {
            return Plugin.Instance?.Configuration.EnablePlugin ?? false;
        }

        public static Uri GetServerUrl()
        {
            var raw = Plugin.Instance?.Configuration.ServerUrl ?? "localhost:8096";

            var hasScheme = raw.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                         || raw.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

            var baseUri = new Uri(hasScheme ? raw : $"http://{raw}", UriKind.Absolute); // Do not remove scheme if present, could lead to issues

            return baseUri;
        }

        public static string GetMaintenanceStartMessage()
        {
            return Plugin.Config.MaintenanceStartStringMessage ?? string.Empty;
        }

        public static string GetMaintenanceEndMessage()
        {
            return Plugin.Config.MaintenanceEndStringMessage ?? string.Empty;
        }

        public static int GetMetadataWaitMultiplier()
        {
            return Plugin.Instance?.Configuration.MetadataWaitMultiplier ?? 10;
        }

        public static int GetPlaybackStartDebounceMs()
        {
            return Plugin.Instance?.Configuration.PlaybackStartDebounceMs ?? 0;
        }
    }
}
