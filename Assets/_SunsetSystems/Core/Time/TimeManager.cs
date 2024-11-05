using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Core.TimeFlow
{
    public class TimeManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private SunriseSunsetData _sunriseSunsetData;

        private DateTime _gameDate = new(1986, 6, 14, 20, 0, 0);
        private DateTime _cachedCycleSunrise;
        private DateTime _cachedCycleSunset;
        [Title("Calendar")]
        [ShowInInspector, ReadOnly]
        private int Year => _gameDate.Year;
        [ShowInInspector, ReadOnly]
        private int Month => _gameDate.Month;
        [ShowInInspector, ReadOnly]
        private int Day => _gameDate.Day;
        [Title("Time Of Day")]
        [ShowInInspector, ReadOnly]
        private int Hour => _gameDate.Hour;
        [ShowInInspector, ReadOnly]
        private int Minute => _gameDate.Minute;
        [ShowInInspector, ReadOnly]
        private int Second => _gameDate.Second;

        public static event Action<DateTime> OnTimeUpdated;
        public static event Action<DateTime> OnDayChanged;

        //public void AddTime(double minutes)
        //{
        //    _gameDate = _gameDate.AddMinutes(minutes);
        //    OnTimeUpdated?.Invoke(_gameDate);
        //}

        //public void AddDate(int days, int months = 0, int years = 0)
        //{
        //    _gameDate = _gameDate.AddDays(days);
        //    _gameDate = _gameDate.AddMonths(months);
        //    _gameDate = _gameDate.AddYears(years);
        //    if (days != 0 || months != 0 || years != 0)
        //        OnDayChanged?.Invoke(_gameDate);
        //}

        private void UpdateSunriseSunsetCache()
        {
            if (IsDay())
            {
                _cachedCycleSunrise = GetLastSunrise();
                _cachedCycleSunset = GetNextSunset();
            }
            else
            {
                _cachedCycleSunrise = GetNextSunrise();
                _cachedCycleSunset = GetLastSunset();
            }
        }

        public bool IsDay()
        {
            var currentTime = GetCurrentTime();
            bool wasLastSunriseToday = IsSameDay(currentTime, GetLastSunrise());
            bool isNextSunsetToday = IsSameDay(currentTime, GetNextSunset());
            return wasLastSunriseToday && isNextSunsetToday;
        }

        public bool IsNight()
        {
            return !IsDay();
        }

        private DateTime GetNextSunset() => GetNextSunset(GetCurrentTime());
        private DateTime GetNextSunset(DateTime currentTime) 
        {
            if (_sunriseSunsetData.TryGetTimeData(currentTime, out var timeData))
            {
                var sunsetTime = timeData.SunsetTime;
                var sunsetDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, sunsetTime.Hours, sunsetTime.Minutes, 0);
                if (sunsetDate > currentTime)
                {
                    return sunsetDate;
                }
                else
                {
                    var nextDay = currentTime.AddDays(1);
                    if (_sunriseSunsetData.TryGetTimeData(nextDay, out timeData))
                    {
                        sunsetTime = timeData.SunsetTime;
                        return new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, sunsetTime.Hours, sunsetTime.Minutes, 0);
                    }
                }
            }
            return DateTime.MaxValue;
        }

        private DateTime GetLastSunset() => GetLastSunset(GetCurrentTime());
        private DateTime GetLastSunset(DateTime currentTime)
        {
            if (_sunriseSunsetData.TryGetTimeData(currentTime, out var timeData))
            {
                var sunsetTime = timeData.SunsetTime;
                var sunsetDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, sunsetTime.Hours, sunsetTime.Minutes, 0);
                if (sunsetDate <= currentTime)
                {
                    return sunsetDate;
                }
                else
                {
                    var previousDay = currentTime.AddDays(-1);
                    if (_sunriseSunsetData.TryGetTimeData(previousDay, out timeData))
                    {
                        sunsetTime = timeData.SunsetTime;
                        return new DateTime(previousDay.Year, previousDay.Month, previousDay.Day, sunsetTime.Hours, sunsetTime.Minutes, 0);
                    }
                }
            }
            return DateTime.MinValue;
        }

        private DateTime GetNextSunrise() => GetNextSunrise(GetCurrentTime());
        private DateTime GetNextSunrise(DateTime currentTime)
        {
            if (_sunriseSunsetData.TryGetTimeData(currentTime, out var timeData))
            {
                var sunriseTime = timeData.SunriseTime;
                var sunriseDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, sunriseTime.Hours, sunriseTime.Minutes, 0);
                if (sunriseDate >= currentTime)
                {
                    return sunriseDate;
                }
                else
                {
                    var nextDay = currentTime.AddDays(1);
                    if (_sunriseSunsetData.TryGetTimeData(nextDay, out timeData))
                    {
                        sunriseTime = timeData.SunriseTime;
                        return new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, sunriseTime.Hours, sunriseTime.Minutes, 0);
                    }
                }
            }
            return DateTime.MaxValue;
        }

        private DateTime GetLastSunrise() => GetLastSunrise(GetCurrentTime());
        private DateTime GetLastSunrise(DateTime currentTime)
        {
            if (_sunriseSunsetData.TryGetTimeData(currentTime, out var timeData))
            {
                var sunriseTime = timeData.SunriseTime;
                var sunriseDate = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, sunriseTime.Hours, sunriseTime.Minutes, 0);
                if (sunriseDate < currentTime)
                {
                    return sunriseDate;
                }
                else
                {
                    var previousDay = currentTime.AddDays(-1);
                    if (_sunriseSunsetData.TryGetTimeData(previousDay, out timeData))
                    {
                        sunriseTime = timeData.SunsetTime;
                        return new DateTime(previousDay.Year, previousDay.Month, previousDay.Day, sunriseTime.Hours, sunriseTime.Minutes, 0);
                    }
                }
            }
            return DateTime.MinValue;
        }

        private bool IsDifferentDay(DateTime timeA, DateTime timeB)
        {
            return timeA.Day != timeB.Day || timeA.Month != timeB.Month || timeA.Year != timeB.Year;
        }

        private bool IsSameDay(DateTime timeA, DateTime timeB)
        {
            return timeA.Day == timeB.Day && timeA.Month == timeB.Month && timeA.Year == timeB.Year;
        }

        public void SetDateTime(DateTime newDateTime) => _gameDate = newDateTime;
        public DateTime GetCurrentTime() => _gameDate;
    }
}
