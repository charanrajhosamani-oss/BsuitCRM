using System.Globalization;

namespace BSuit.Infrastructure.Common
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime Now(string timezoneId)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        }

        public DateTime ConvertToUtc(DateTime dateTime, string timezoneId)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            return TimeZoneInfo.ConvertTimeToUtc(dateTime, tz);
        }

        public DateTime ConvertFromUtc(DateTime utcDateTime, string timezoneId)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
        }

        public string Format(DateTime dateTime, string format = "dd-MM-yyyy HH:mm:ss")
        {
            return dateTime.ToString(format);
        }

        public DateTime Parse(string dateTime, string format)
        {
            return DateTime.ParseExact(dateTime, format, CultureInfo.InvariantCulture);
        }

        public int GetAge(DateTime dob)
        {
            var today = DateTime.Today;
            int age = today.Year - dob.Year;

            if (dob.Date > today.AddYears(-age))
                age--;

            return age;
        }

        public DateTime StartOfDay(DateTime date) => date.Date;

        public DateTime EndOfDay(DateTime date) => date.Date.AddDays(1).AddTicks(-1);

        public DateTime StartOfWeek(DateTime date, DayOfWeek startDay = DayOfWeek.Monday)
        {
            int diff = (7 + (date.DayOfWeek - startDay)) % 7;
            return date.AddDays(-diff).Date;
        }

        public DateTime EndOfWeek(DateTime date, DayOfWeek startDay = DayOfWeek.Monday)
        {
            return StartOfWeek(date, startDay).AddDays(7).AddTicks(-1);
        }

        public DateTime StartOfMonth(DateTime date)
            => new DateTime(date.Year, date.Month, 1);

        public DateTime EndOfMonth(DateTime date)
            => StartOfMonth(date).AddMonths(1).AddTicks(-1);

        public DateTime StartOfYear(DateTime date)
            => new DateTime(date.Year, 1, 1);

        public DateTime EndOfYear(DateTime date)
            => new DateTime(date.Year, 12, 31).AddDays(1).AddTicks(-1);

        public bool IsWeekend(DateTime date)
            => date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

        public bool IsToday(DateTime date)
            => date.Date == DateTime.Today;

        public int GetBusinessDays(DateTime start, DateTime end)
        {
            int count = 0;

            for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
            {
                if (!IsWeekend(date))
                    count++;
            }

            return count;
        }

        public string ToRelativeTime(DateTime dateTime)
        {
            var span = DateTime.UtcNow - dateTime;

            if (span.TotalSeconds < 60)
                return $"{span.Seconds} seconds ago";

            if (span.TotalMinutes < 60)
                return $"{span.Minutes} minutes ago";

            if (span.TotalHours < 24)
                return $"{span.Hours} hours ago";

            if (span.TotalDays < 30)
                return $"{span.Days} days ago";

            return dateTime.ToString("dd MMM yyyy");
        }

        public long ToUnixTimestamp(DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        public DateTime FromUnixTimestamp(long timestamp)
        {
            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        }
    }
}
