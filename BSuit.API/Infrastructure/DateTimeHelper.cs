using Microsoft.AspNetCore.Mvc.Rendering;
using System;

//6. Install Cross-Platform Package
using TimeZoneConverter;

namespace BSuit.API.Infrastructure
{
    /// <summary>
    /// 5. Universal Helper
    /// </summary>
    public static class DateTimeHelper
    {
        public static DateTime ToUserTime(
            DateTime utcDate,
            HttpContext httpContext)
        {
            string timeZoneId =
                httpContext.Session.GetString("UserTimeZone");

            if (string.IsNullOrWhiteSpace(timeZoneId))
                return utcDate;

            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(
                    TZConvert.IanaToWindows(timeZoneId));

                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.SpecifyKind(
                        utcDate,
                        DateTimeKind.Utc),
                    tz);
            }
            catch
            {
                return utcDate;
            }
        }



        /// <summary>
        /// builder.Services.AddHttpContextAccessor();
        /// ---------------------------------------------------
        /// @inject IHttpContextAccessor HttpContextAccessor
        /// @item.CreatedOn.ToUserTime(HttpContextAccessor)
        /// ---------------------------------------------------
        /// </summary>
        /// <param name="utcDate"></param>
        /// <param name="accessor"></param>
        /// <returns></returns>
        public static DateTime ToUserTime(
          this DateTime utcDate,
          IHttpContextAccessor accessor)
        {
            if (utcDate == DateTime.MinValue)
                return utcDate;

            var context = accessor.HttpContext;

            if (context == null)
                return utcDate;

            var timeZone = context.Session
                .GetString("UserTimeZone");

            if (string.IsNullOrWhiteSpace(timeZone))
                return utcDate;

            try
            {
                var windowsTimeZone =
                    TZConvert.IanaToWindows(timeZone);

                var tz = TimeZoneInfo
                    .FindSystemTimeZoneById(windowsTimeZone);

                return TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.SpecifyKind(
                        utcDate,
                        DateTimeKind.Utc),
                    tz);
            }
            catch
            {
                return utcDate;
            }
        }


        /// <summary>
        /// @Html.ToLocalDate(item.CreatedOn)
        /// </summary>
        /// <param name="html"></param>
        /// <param name="utcDate"></param>
        /// <returns></returns>
        public static string ToLocalDate(
       this IHtmlHelper html,
       DateTime utcDate)
        {
            var accessor =
                html.ViewContext.HttpContext
                    .RequestServices
                    .GetService<IHttpContextAccessor>();

            return utcDate
                .ToUserTime(accessor)
                .ToString("dd-MMM-yyyy hh:mm tt");
        }


    }
}
