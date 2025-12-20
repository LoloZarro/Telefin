using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telefin.Common.Enums;
using Telefin.Common.Extensions;
using Telefin.NotificationContext;
using Telefin.NotificationContext.Interface;

namespace Telefin.Helper
{
    public class NotificationDispatcher
    {
        private readonly ILogger<NotificationDispatcher> _logger;
        private readonly TelegramSender _sender;

        public NotificationDispatcher(ILogger<NotificationDispatcher> logger, TelegramSender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        public async Task DispatchNotificationsAsync(NotificationType notificationType, dynamic item, string userId = "", string subtype = "")
        {
            if (!Plugin.Config.EnablePlugin)
            {
                return;
            }

            var userConfigurations = Plugin.Config.UserConfigurations;
            var tasks = new List<Task>();

            foreach (var userConfiguration in userConfigurations)
            {
                if (!userConfiguration.EnableUser)
                {
                    continue;
                }

                var isNotificationTypeEnabled = userConfiguration.GetPropertySafely<bool?>(notificationType.ToString());
                if (isNotificationTypeEnabled != true)
                {
                    continue;
                }

                if (userConfiguration.DoNotMentionOwnActivities && userConfiguration.UserId is not null)
                {
                    var currentUserid = userConfiguration.UserId.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
                    var notifUserId = userId.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
                    if (currentUserid == notifUserId)
                    {
                        continue;
                    }
                }

                string? message;

                if (!string.IsNullOrEmpty(subtype))
                {
                    var isSubTypeEnabled = userConfiguration.GetPropertySafely<bool?>(subtype);
                    if (isSubTypeEnabled != true)
                    {
                        continue;
                    }

                    message = userConfiguration.GetPropertySafely<string?>(subtype + "StringMessage");
                }
                else
                {
                    message = userConfiguration.GetPropertySafely<string?>(notificationType.ToString() + "StringMessage");
                }

                var itemType = item.GetType().ToString();

                INotificationContext? context = CreateNotificationTypeContext(notificationType, subtype, item);
                //if (context == null)
                //{
                //    _logger.LogError("{PluginName}: Unable to dispatch notification for {ItemName} there is no corresponding NotificationTypeContext.", typeof(Plugin).Name, (string)itemType);
                //    return;
                //}

                var data = context?.GetTemplateData();
                message = PlaceholderReplacer.Resolve(message, data);

                if (string.IsNullOrEmpty(message))
                {
                    _logger.LogError("{PluginName}: Unable to dispatch notification for {ItemName} as the message is empty.", typeof(Plugin).Name, (string)itemType);
                    return;
                }

                var botToken = userConfiguration.BotToken;
                var chatId = userConfiguration.ChatId;
                var isSilentNotification = userConfiguration.SilentNotification;
                var threadId = userConfiguration.ThreadId;

                try
                {
                    var imagePath = context?.GetImagePath();
                    if (string.IsNullOrWhiteSpace(imagePath))
                    {
                        Task task = _sender.SendMessageAsync(notificationType.ToString(), message, botToken, chatId, isSilentNotification, threadId);
                        tasks.Add(task);
                    }
                    else
                    {
                        Task task = _sender.SendMessageWithPhotoAsync(notificationType.ToString(), message, imagePath, botToken, chatId, isSilentNotification, threadId);
                        tasks.Add(task);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("{PluginName}: An error occurred while sending a Telegram message: {ExceptionMessage}", typeof(Plugin).Name, ex.Message);
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private static INotificationContext? CreateNotificationTypeContext(NotificationType type, string? subtype, dynamic eventArgs)
        {
            return type switch
            {
                NotificationType.PlaybackStart => new PlaybackContext(eventArgs),
                NotificationType.PlaybackStop => new PlaybackContext(eventArgs),
                NotificationType.PlaybackProgress => new PlaybackContext(eventArgs),

                NotificationType.ItemAdded => new LibraryItemContext(eventArgs),
                NotificationType.ItemDeleted => new LibraryItemContext(eventArgs),

                NotificationType.AuthenticationFailure => new AuthenticationRequestContext(eventArgs),
                NotificationType.AuthenticationSuccess => new AuthenticationResultContext(eventArgs),

                NotificationType.PluginInstalled => new PluginInstallInfoContext(eventArgs),
                NotificationType.PluginInstalling => new PluginInstallInfoContext(eventArgs),
                NotificationType.PluginUpdated => new PluginInstallInfoContext(eventArgs),
                NotificationType.PluginInstallationCancelled => new PluginInstallInfoContext(eventArgs),
                NotificationType.PluginInstallationFailed => new PluginInstallationFailedContext(eventArgs),
                NotificationType.PluginUninstalled => new PluginUninstalledContext(eventArgs),

                NotificationType.SubtitleDownloadFailure => new SubtitleDownloadFailureContext(eventArgs),
                NotificationType.TaskCompleted => new TaskCompletedConext(eventArgs),

                NotificationType.UserCreated => new GenericUserContext(eventArgs),
                NotificationType.UserDeleted=> new GenericUserContext(eventArgs),
                NotificationType.UserUpdated => new GenericUserContext(eventArgs),
                NotificationType.UserPasswordChanged => new GenericUserContext(eventArgs),
                NotificationType.UserLockedOut=> new GenericUserContext(eventArgs),

                _ => null
            };
        }
    }
}