using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telefin.Common.Extensions;
using Telefin.Common.Models;

namespace Telefin.Helper
{
    public class TelegramSender
    {
        private const int MaxMessageLength = 4096;
        private const int MaxCaptionLength = 1024;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly HttpClient _httpClient;
        private readonly ILogger<Plugin> _logger;

        public TelegramSender(HttpClient httpClient, ILogger<Plugin> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> SendMessageAsync(string notificationType, string message, string botToken, string chatId, bool isSilentNotification, string threadId)
        {
            try
            {
                var endpoint = BuildBotUrl(botToken, "sendMessage");

                var messages = message.SplitMessage(MaxMessageLength).ToArray();

                var parameters = new Dictionary<string, string>
                {
                    { "chat_id", chatId },
                    { "parse_mode", "HTML" }
                };

                if (isSilentNotification)
                {
                    parameters.Add("disable_notification", "true");
                }

                if (SanitizeThreadId(threadId, out var sanitizedThreadId))
                {
                    parameters.Add("message_thread_id", sanitizedThreadId);
                }

                bool success = false;

                foreach (var msg in messages)
                {
                    parameters["text"] = msg;

                    success = await PostFormAsync(notificationType, endpoint, parameters).ConfigureAwait(false);
                    if (!success)
                    {
                        _logger.LogError("{PluginName}({NotificationType}): Error while trying to dispatch Telegram Notification. Full Message:{Message}", Plugin.PluginName, notificationType, message);
                        break;
                    }
                }

                return success;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "{PluginName}({NotificationType}): Invalid bot token provided.", Plugin.PluginName, notificationType);
                return false;
            }
        }

        public async Task<bool> SendMessageWithPhotoAsync(string notificationType, string caption, string imageUrl, string botToken, string chatId, bool isSilentNotification, string threadId)
        {
            try
            {
                var endpoint = BuildBotUrl(botToken, "sendPhoto");

                var messages = caption.SplitMessage(MaxCaptionLength).ToArray();

                using var form = new MultipartFormDataContent
                {
                    { new StringContent(chatId, Encoding.UTF8), "chat_id" },
                    { new StringContent(messages.FirstOrDefault() ?? string.Empty, Encoding.UTF8), "caption" },
                    { new StringContent("HTML", Encoding.UTF8), "parse_mode" }
                };

                if (isSilentNotification)
                {
                    form.Add(new StringContent("true", Encoding.UTF8), "disable_notification");
                }

                if (SanitizeThreadId(threadId, out var sanitizedThreadId))
                {
                    form.Add(new StringContent(sanitizedThreadId, Encoding.UTF8), "message_thread_id");
                }

                // Fetch image
                try
                {
                    _logger.LogDebug("{PluginName}({NotificationType}): Fetching image: {Url}", Plugin.PluginName, notificationType, imageUrl);

                    using var imageResponse = await _httpClient.GetAsync(imageUrl).ConfigureAwait(false);
                    var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                    if (!imageResponse.IsSuccessStatusCode || imageBytes.Length == 0)
                    {
                        _logger.LogError("{PluginName}({NotificationType}): Failed to fetch image. HTTP {StatusCode}", Plugin.PluginName, notificationType, (int)imageResponse.StatusCode);
                        return false;
                    }

                    var imageContent = new ByteArrayContent(imageBytes);
                    // Telegram doesn’t require correct extension, but it helps
                    form.Add(imageContent, "photo", "image.jpg");
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
                {
                    _logger.LogWarning(ex, "{PluginName}({NotificationType}): Error fetching image: {Url}. Falling back to regular message.", Plugin.PluginName, notificationType, imageUrl);
                    return await SendMessageAsync(notificationType, caption ?? string.Empty, botToken, chatId, isSilentNotification, threadId).ConfigureAwait(false);
                }

                var success = await PostMultipartAsync(notificationType, endpoint, form).ConfigureAwait(false);
                if (!success)
                {
                    _logger.LogError("{PluginName}({NotificationType}): Error while trying to dispatch Telegram Notification. Full Message:{Message}", Plugin.PluginName, notificationType, caption);
                    return false;
                }

                if (messages.Skip(1).Any())
                {
                    success = await SendMessageAsync(notificationType, string.Join(string.Empty, messages.Skip(1)) ?? string.Empty, botToken, chatId, isSilentNotification, threadId).ConfigureAwait(false);
                }

                return success;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "{PluginName}({NotificationType}): Invalid bot token provided.", Plugin.PluginName, notificationType);
                return false;
            }
        }

        private async Task<bool> PostFormAsync(string notificationType, string url, Dictionary<string, string> parameters)
        {
            try
            {
                using var content = new FormUrlEncodedContent(parameters);
                using var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false);
                return await HandleTelegramResponseAsync(notificationType, response).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                _logger.LogError(ex, "{PluginName}({NotificationType}): Request failed. Please check configuration and connectivity.", Plugin.PluginName, notificationType);
                return false;
            }
        }

        private async Task<bool> PostMultipartAsync(string notificationType, string url, MultipartFormDataContent form)
        {
            try
            {
                using var response = await _httpClient.PostAsync(url, form).ConfigureAwait(false);
                return await HandleTelegramResponseAsync(notificationType, response).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                _logger.LogError(ex, "{PluginName}({NotificationType}): Request failed. Please check configuration and connectivity.", Plugin.PluginName, notificationType);
                return false;
            }
        }

        private async Task<bool> HandleTelegramResponseAsync(string notificationType, HttpResponseMessage response)
        {
            var body = string.Empty;

            try
            {
                body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                // Telegram often returns 200 even when ok=false
                TelegramResponse? parsed = null;
                if (!string.IsNullOrWhiteSpace(body))
                {
                    parsed = JsonSerializer.Deserialize<TelegramResponse>(body, JsonOptions);
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "{PluginName}({NotificationType}): Telegram HTTP error {StatusCode}. Body: {Body}",
                        Plugin.PluginName,
                        notificationType,
                        (int)response.StatusCode,
                        body);
                    return false;
                }

                if (parsed is not null && parsed.Ok == false)
                {
                    _logger.LogError(
                        "{PluginName}({NotificationType}): Telegram API error {ErrorCode}: {Description}. Body: {Body}",
                        Plugin.PluginName,
                        notificationType,
                        parsed.ErrorCode,
                        parsed.Description,
                        body);

                    return false;
                }

                _logger.LogDebug("{PluginName}({NotificationType}): Message sent successfully.", Plugin.PluginName, notificationType);
                return true;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "{PluginName}({NotificationType}): Could not parse Telegram response. HTTP {StatusCode}. Body: {Body}", Plugin.PluginName, notificationType, (int)response.StatusCode, body);
                return false;
            }
        }

        private static string BuildBotUrl(string botToken, string method)
        {
            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("Bot token is required.", nameof(botToken));
            }

            return $"https://api.telegram.org/bot{botToken}/{method}";
        }

        // Telegram expects an integer for the thread id. UI stores it as string -> sanitize safely.
        private static bool SanitizeThreadId(string threadId, out string sanitize)
        {
            sanitize = string.Empty;

            if (string.IsNullOrWhiteSpace(threadId))
            {
                return false;
            }

            if (int.TryParse(threadId, out var id) && id > 0)
            {
                sanitize = id.ToString(System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }
    }
}