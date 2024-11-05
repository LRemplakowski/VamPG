using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SunsetSystems.Core.TimeFlow
{
#if UNITY_EDITOR
    public static class TimeDataCSVImporter
    {
        public static Dictionary<DayOfYear, SunsetSunriseTime> ImportCSV(string filePath)
        {
            var sunsetSunriseData = new Dictionary<DayOfYear, SunsetSunriseTime>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("CSV file not found: " + filePath);
            }

            string[] lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++) // Start from 1 to skip the header
            {
                string[] values = lines[i].Split(',');

                DateTime date = DateTime.ParseExact(values[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                int sunriseHour, sunriseMinute, sunsetHour, sunsetMinute;

                TimeUtility.ConvertTo24HourFormat(values[1], out sunriseHour, out sunriseMinute);
                TimeUtility.ConvertTo24HourFormat(values[2], out sunsetHour, out sunsetMinute);

                var dayOfYear = new DayOfYear
                {
                    Year = (short)date.Year,
                    Month = (byte)date.Month,
                    Day = (byte)date.Day
                };

                var timeOfDay = new SunsetSunriseTime
                {
                    SunriseTime = new TimeOfDay { Hours = (byte)sunriseHour, Minutes = (byte)sunriseMinute },
                    SunsetTime = new TimeOfDay { Hours = (byte)sunsetHour, Minutes = (byte)sunsetMinute }
                };

                sunsetSunriseData[dayOfYear] = timeOfDay;
            }

            return sunsetSunriseData;
        }
    }
#endif
}
