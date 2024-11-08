using System;
using SunsetSystems.Core.TimeFlow;

namespace SunsetSystems.WorldMap
{
    public static class WorldMapTravelTimeCalculator
    {
        public static void CalculateTravelTime(in WorldMapTravelContext travelContext, out TravelTimeData travelData)
        {
            TimeSpan travelDuration = TimeSpan.FromHours(travelContext.TravelDistance / travelContext.TravelSpeed);
            TimeSpan returnDuration = TimeSpan.FromHours(travelContext.ReturnDistanceToPlayerHaven / travelContext.TravelSpeed);
            var timeManager = TimeManager.Instance;
            bool willArriveAfterSunrise = true;
            bool canReturnBeforeSunrise = false;
            if (timeManager.GetCachedIsNight())
            {
                TimeSpan timeToSunrise = timeManager.GetTimeToSunrise();
                willArriveAfterSunrise = timeToSunrise < travelDuration;
                canReturnBeforeSunrise = timeToSunrise < travelDuration + returnDuration;
            }
            travelData = new(travelDuration, willArriveAfterSunrise, canReturnBeforeSunrise);
        }
    }

    public readonly struct WorldMapTravelContext
    {
        public readonly float TravelDistance;
        public readonly float ReturnDistanceToPlayerHaven;
        public readonly float TravelSpeed;

        public WorldMapTravelContext(IWorldMapManager worldMapManager, IWorldMapData start, IWorldMapData end)
        {
            TravelDistance = worldMapManager.GetDistanceBetweenAreas(start, end);
            ReturnDistanceToPlayerHaven = worldMapManager.GetDistanceBetweenAreas(end, worldMapManager.GetPlayerHavenMap());
            TravelSpeed = worldMapManager.GetPlayerTravelSpeed();
        }
    }

    public readonly struct TravelTimeData
    {
        public readonly TimeSpan TravelDuration;
        public readonly bool WillArriveAfterSunrise, CanReturnBeforeSunrise;

        public TravelTimeData(TimeSpan travelDuration, bool willArriveAfterSunrise, bool canReturnBeforeSunrise)
        {
            TravelDuration = travelDuration;
            WillArriveAfterSunrise = willArriveAfterSunrise;
            CanReturnBeforeSunrise = canReturnBeforeSunrise;
        }
    }
}
