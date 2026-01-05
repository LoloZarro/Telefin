using System;
using System.Collections.Generic;

namespace Telefin.Helper
{
    public static class PlaceholderReplacer
    {
        public static string? Resolve(string? template, IDictionary<string, string?>? data)
        {
            if (string.IsNullOrWhiteSpace(template) || data == null || data.Count == 0)
            {
                return template ?? null;
            }

            var message = template;

            foreach (var kv in data)
            {
                var key = kv.Key;
                var value = string.IsNullOrEmpty(kv.Value) ? "N/A" : kv.Value;

                if (message.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    message = message.Replace(key, value, StringComparison.OrdinalIgnoreCase);
                }
            }

            return message;
        }
    }
}