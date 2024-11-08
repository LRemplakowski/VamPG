using System;
using System.Globalization;

namespace SunsetSystems.Core.TimeFlow
{
    public static class TimeUtility
    {
        public static void ConvertTo24HourFormat(string time12Hour, out int hours, out int minutes)
        {
            DateTime dateTime = DateTime.ParseExact(time12Hour, "h:mm tt", CultureInfo.InvariantCulture);
            hours = dateTime.Hour;
            minutes = dateTime.Minute;
        }

        public static void ConvertTo12HourFormat(string time24Hour, out string time12Hour)
        {
            DateTime dateTime = DateTime.ParseExact(time24Hour, "HH:mm", CultureInfo.InvariantCulture);
            time12Hour = dateTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
        }

        public static void ConvertTo12HourFormat(int hours, int minutes, out string time12Hour)
        {
            DateTime dateTime = new(1, 1, 1, hours, minutes, 0);
            time12Hour = dateTime.ToString("h:mm tt", CultureInfo.InvariantCulture);
        }

        public static TimeSpan GetTimeSpanBetweenDates(in DateTime dateA, in DateTime dateB)
        {
            if (dateA > dateB)
            {
                return dateA - dateB;
            }
            else if (dateB > dateA)
            {
                return dateB - dateA;
            }
            else
            {
                return TimeSpan.Zero;
            }
        }
    }
}
