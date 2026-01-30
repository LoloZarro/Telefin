using System.Collections.Generic;
using System.Globalization;
using MediaBrowser.Model.Tasks;

namespace Telefin.NotificationContext
{
    internal class TaskCompletedConext : NotificationContextBase
    {
        private readonly TaskCompletionEventArgs _eventArgs;

        public TaskCompletedConext(TaskCompletionEventArgs item)
        {
            _eventArgs = item;
        }

        public override IDictionary<string, string?> GetTemplateData()
        {
            var data = new Dictionary<string, string?>();

            var result = _eventArgs?.Result;
            var task = _eventArgs?.Task;

            // Current run
            data["{status}"] = result?.Status.ToString();
            data["{name}"] = result?.Name ?? task?.Name;
            data["{key}"] = result?.Key;
            data["{id}"] = result?.Id;
            data["{error}"] = result?.ErrorMessage;
            data["{fullError}"] = result?.LongErrorMessage;

            data["{startTimeUtc}"] = result?.StartTimeUtc.ToString("o", CultureInfo.InvariantCulture);
            data["{endTimeUtc}"] = result?.EndTimeUtc.ToString("o", CultureInfo.InvariantCulture);

            var duration = result?.EndTimeUtc - result?.StartTimeUtc;
            data["{durationRaw}"] = duration.ToString();
            data["{durationSeconds}"] = duration?.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            data["{durationMinutes}"] = duration?.TotalMinutes.ToString(CultureInfo.InvariantCulture);

            // Scheduled Task
            data["{taskName}"] = task?.Name;
            data["{taskDescription}"] = task?.Description;
            data["{taskCategory}"] = task?.Category;
            data["{taskId}"] = task?.Id;
            data["{taskState}"] = task?.State.ToString();
            data["{taskProgress}"] = task?.CurrentProgress?.ToString(CultureInfo.InvariantCulture);

            return data;
        }
    }
}
