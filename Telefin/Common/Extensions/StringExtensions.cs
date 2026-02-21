using System;
using System.Collections.Generic;

namespace Telefin.Common.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitMessage(this string message, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(message) || message.Length <= maxLength)
            {
                yield return message;
                yield break;
            }

            int start = 0;
            while (start < message.Length)
            {
                int length = Math.Min(maxLength, message.Length - start);
                int end = start + length;

                if (end < message.Length && (message[end] != '\n' || message[end] != ' '))
                {
                    int lastTruncateCharacter = message.LastIndexOf('\n', end); // Prefer linbreaks
                    if (lastTruncateCharacter == -1)
                    {
                        lastTruncateCharacter = message.LastIndexOf(' ', end);
                    }

                    if (lastTruncateCharacter > start)
                    {
                        length = lastTruncateCharacter - start + 1;
                    }
                }

                yield return message.Substring(start, length);
                start += length;
            }
        }

        public static string Sanitize(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            return text.Replace("<br>", "\r\n", StringComparison.OrdinalIgnoreCase);
        }
    }
}
