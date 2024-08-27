using System;

namespace SOS.OrderTracking.Web.Shared
{
    public static class MyDateTime
    {
        static TimeZoneInfo _cetZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");

        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _cetZone);

        public static DateTime Today => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _cetZone).Date;

        public static DateTime ToPsTime(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, _cetZone).Date;
        }
    }
}
