using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telefin.Common.Enums;
using Telefin.Configuration;

namespace Telefin.Helper
{
    public class NotificationDispatcher
    {
        private readonly TelegramSender _sender;

        public NotificationDispatcher(TelegramSender sender)
        {
            _sender = sender;
        }

        public async Task DispatchNotificationsAsync(NotificationType type, dynamic eventArgs, string userId = "", string imagePath = "", string subtype = "")
        {
            if (!Plugin.Config.EnablePlugin)
            {
                return;
            }

            var users = Plugin.Config.UserConfigurations;
            var tasks = new List<Task>();

            foreach (var user in users)
            {
                if (!user.EnableUser)
                {
                    continue;
                }

                var isNotificationTypeEnabled = GetPropertyValue(user, type.ToString());
                if (!isNotificationTypeEnabled)
                {
                    continue;
                }

                if (user.DoNotMentionOwnActivities && user.UserId is not null)
                {
                    var currentUserid = user.UserId.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
                    var notifUserId = userId.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);
                    if (currentUserid == notifUserId)
                    {
                        continue;
                    }
                }

                string message;

                if (!string.IsNullOrEmpty(subtype))
                {
                    var isSubTypeEnabled = GetPropertyValue(user, subtype);
                    if (!isSubTypeEnabled)
                    {
                        continue;
                    }

                    message = GetPropertyMessage(user, subtype);
                }
                else
                {
                    message = GetPropertyMessage(user, type.ToString());
                }

                message = MessageParser.ParseMessage(message, eventArgs);

                var botToken = user.BotToken;
                var chatId = user.ChatId;
                var isSilentNotification = user.SilentNotification;
                var threadId = user.ThreadId;

                try
                {
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        Task task = _sender.SendMessageAsync(type.ToString(), message, botToken, chatId, isSilentNotification, threadId);
                        tasks.Add(task);
                    }
                    else
                    {
                        Task task = _sender.SendMessageWithPhotoAsync(type.ToString(), message, imagePath, botToken, chatId, isSilentNotification, threadId);
                        tasks.Add(task);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while sending a message: {ex.Message}");
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private static T GetProperty<T>(UserConfiguration user, string propertyName)
        {
            var property = typeof(UserConfiguration).GetProperty(propertyName) ?? throw new ArgumentException($"The property '{propertyName}' does not exist.");

            if (!typeof(T).IsAssignableFrom(property.PropertyType))
            {
                throw new ArgumentException($"The property '{propertyName}' is not of type {typeof(T).Name}.");
            }

            return (T)(property.GetValue(user) ?? throw new ArgumentException($"The property '{propertyName}' is null."));
        }

        private bool GetPropertyValue(UserConfiguration user, string propertyName) => GetProperty<bool>(user, propertyName);

        private string GetPropertyMessage(UserConfiguration user, string propertyName) => GetProperty<string>(user, propertyName + "StringMessage");
    }
}