using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.Common
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
        DateTime Now(string timezoneId);

        DateTime ConvertToUtc(DateTime dateTime, string timezoneId);
        DateTime ConvertFromUtc(DateTime utcDateTime, string timezoneId);

        string Format(DateTime dateTime, string format = "dd-MM-yyyy HH:mm:ss");
        DateTime Parse(string dateTime, string format);

        int GetAge(DateTime dateOfBirth);

        DateTime StartOfDay(DateTime date);
        DateTime EndOfDay(DateTime date);

        DateTime StartOfWeek(DateTime date, DayOfWeek startDay = DayOfWeek.Monday);
        DateTime EndOfWeek(DateTime date, DayOfWeek startDay = DayOfWeek.Monday);

        DateTime StartOfMonth(DateTime date);
        DateTime EndOfMonth(DateTime date);

        DateTime StartOfYear(DateTime date);
        DateTime EndOfYear(DateTime date);

        bool IsWeekend(DateTime date);
        bool IsToday(DateTime date);

        int GetBusinessDays(DateTime start, DateTime end);

        string ToRelativeTime(DateTime dateTime);

        long ToUnixTimestamp(DateTime dateTime);
        DateTime FromUnixTimestamp(long timestamp);
    }
}
