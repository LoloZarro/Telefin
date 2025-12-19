using System;
using System.Threading.Tasks;
using Telefin.Common.Enums;
using Telefin.Helper;
using MediaBrowser.Controller.Events;
using MediaBrowser.Model.Tasks;

namespace Telefin.Notifiers;

public class TaskCompletedNotifier : IEventConsumer<TaskCompletionEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public TaskCompletedNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(TaskCompletionEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.TaskCompleted, eventArgs).ConfigureAwait(false);
    }
}