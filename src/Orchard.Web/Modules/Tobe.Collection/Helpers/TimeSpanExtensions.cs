using System;

namespace Tobe.Collection.Helpers
{
    public static class TimeSpanExtensions
    {
        public static string FormatMinutesAndSeconds(this TimeSpan? value)
        {
            return value == null
                ? ""
                : String.Format("{0}:{1:00}", Math.Floor(value.Value.TotalMinutes), value.Value.Seconds);
        }

        public static TimeSpan? ParseMinutesAndSeconds(this string duration)
        {
            if (String.IsNullOrWhiteSpace(duration))
                return null;

            var parts = duration.Split(':');
            if (parts.Length != 2)
                return null;

            var minutes = Int32.Parse(parts[0]);
            var seconds = Int32.Parse(parts[1]);
            var totalSeconds = (minutes*60) + seconds;
            var timeSpan = TimeSpan.FromSeconds(totalSeconds);

            return timeSpan;
        }
    }
}