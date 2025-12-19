using System;
using System.Threading.Tasks;
using Telefin.Common.Enums;
using Telefin.Helper;
using MediaBrowser.Controller.Events;
using MediaBrowser.Controller.Subtitles;

namespace Telefin.Notifiers;

public class SubtitleDownloadFailureNotifier : IEventConsumer<SubtitleDownloadFailureEventArgs>
{
    private readonly NotificationDispatcher _notificationDispatcher;

    public SubtitleDownloadFailureNotifier(NotificationDispatcher notificationFilter)
    {
        _notificationDispatcher = notificationFilter;
    }

    public async Task OnEvent(SubtitleDownloadFailureEventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            throw new ArgumentNullException(nameof(eventArgs));
        }

        await _notificationDispatcher.DispatchNotificationsAsync(NotificationType.SubtitleDownloadFailure, eventArgs).ConfigureAwait(false);
    }
}