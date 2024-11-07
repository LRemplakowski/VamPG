using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Core.TimeFlow
{
    public class TimeManager : SerializedMonoBehaviour
    {
        [SerializeField]
        private bool _progressTimeOnUpdate = false;
        [SerializeField, ShowIf("_progressTimeOnUpdate")]
        private float _timeFlowRate = 1f;
        [SerializeField]
        private SunriseSunsetData _sunriseSunsetData;

        private DateTime _gameDate = new(1986, 6, 14, 20, 0, 0);
        private DateTime _cachedCycleSunrise;
        private DateTime _cachedCycleSunset;
        private TimeUpdateData _cachedTimeUpdate;
        [Title("Day Night Cycle")]
        [ShowInInspector, ReadOnly, LabelText("Is Day")]
        private bool _cachedIsDay;

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

        public delegate void TimeUpdateDelegate(in TimeUpdateData timeData);
        public static event TimeUpdateDelegate OnTimeUpdated;

        public readonly struct TimeUpdateData
        {
            public readonly DateTime CurrentTime;
            public readonly bool IsDay;
            public readonly DateTime SunriseTime;
            public readonly DateTime SunsetTime;

            public TimeUpdateData(DateTime currentTime, bool isDay, DateTime sunriseTime, DateTime sunsetTime)
            {
                CurrentTime = currentTime;
                IsDay = isDay;
                SunriseTime = sunriseTime;
                SunsetTime = sunsetTime;
            }
        }

        private void Start()
        {
            UpdateSunriseSunsetCache();
            _cachedTimeUpdate = new(GetCurrentTime(), GetCachedIsDay(), GetCachedSunriseTime(), GetCachedSunsetTime());
            OnTimeUpdated?.Invoke(in _cachedTimeUpdate);
        }

        private void Update()
        {
            if (_progressTimeOnUpdate)
                AddTime(0, 0, Time.deltaTime * _timeFlowRate);
        }

        [BoxGroup("Editor Utility")]
        [Button]
        public void AddDate(int days, int months, int years)
        {
            var updatedTime = GetCurrentTime();
            if (days > 0)
                updatedTime = updatedTime.AddDays(days);
            if (months > 0)
                updatedTime = updatedTime.AddMonths(months);
            if (years > 0)
                updatedTime = updatedTime.AddYears(years);
            SetCurrentTime(updatedTime);
        }

        [BoxGroup("Editor Utility")]
        [Button]
        public void AddTime(double hours, double minutes, double seconds)
        {
            var updatedTime = GetCurrentTime();
            if (hours > 0)
                updatedTime = updatedTime.AddHours(hours);
            if (minutes > 0)
                updatedTime = updatedTime.AddMinutes(minutes);
            if (seconds > 0)
                updatedTime = updatedTime.AddSeconds(seconds);
            SetCurrentTime(updatedTime);
        }

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

        public bool GetCachedIsDay() => _cachedIsDay;
        private bool IsDay()
        {
            var currentTime = GetCurrentTime();
            bool wasLastSunriseToday = IsSameDay(currentTime, GetLastSunrise());
            bool isNextSunsetToday = IsSameDay(currentTime, GetNextSunset());
            _cachedIsDay = wasLastSunriseToday && isNextSunsetToday;
            return GetCachedIsDay();
        }

        public bool GetCachedIsNight() => !GetCachedIsDay();
        private bool IsNight()
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
            return !IsSameDay(timeA, timeB);
        }

        private bool IsSameDay(DateTime timeA, DateTime timeB)
        {
            return timeA.Day == timeB.Day && timeA.Month == timeB.Month && timeA.Year == timeB.Year;
        }

        private bool IsCurrentTimePastDayNightTreshold()
        {
            return GetCachedIsDay() && GetCurrentTime() > GetCachedSunsetTime() || GetCachedIsNight() && GetCurrentTime() > GetCachedSunriseTime();
        }

        public void SetCurrentTime(DateTime newDateTime) => SetCurrentTime(newDateTime, false);
        public void SetCurrentTime(DateTime newDateTime, bool preventTimeUpdateEvent)
        {
            if (newDateTime == GetCurrentTime())
                return;
            var previousDate = _gameDate;
            _gameDate = newDateTime;
            if (IsCurrentTimePastDayNightTreshold() || IsDifferentDay(_gameDate, previousDate))
                UpdateSunriseSunsetCache();
            _cachedTimeUpdate = new TimeUpdateData(GetCurrentTime(), GetCachedIsDay(), GetCachedSunriseTime(), GetCachedSunsetTime());
            if (!preventTimeUpdateEvent)
                OnTimeUpdated?.Invoke(in _cachedTimeUpdate);
        }

        public DateTime GetCurrentTime() => _gameDate;
        public DateTime GetCachedSunriseTime() => _cachedCycleSunrise;
        public DateTime GetCachedSunsetTime() => _cachedCycleSunset;
    }
}
