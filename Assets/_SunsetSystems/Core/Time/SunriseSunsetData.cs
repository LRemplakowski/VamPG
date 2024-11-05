using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SunsetSystems.Core.TimeFlow
{
    [CreateAssetMenu(fileName = "New Sunrise Sunset Data Asset", menuName = "Sunset Core/Sunrise Sunset Data")]
    public class SunriseSunsetData : SerializedScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField, FilePath]
        private string _importDataPath = "";
#endif
        [SerializeField]
        private Dictionary<DayOfYear, SunsetSunriseTime> _sunsetSunriseData = new();

        public bool TryGetTimeData(DateTime dateTime, out SunsetSunriseTime timeData)
        {
            return TryGetTimeData(new DayOfYear(dateTime.Year, dateTime.Month, dateTime.Day), out timeData);
        }

        public bool TryGetTimeData(int year, int month, int day, out SunsetSunriseTime timeData)
        {
            return TryGetTimeData(new DayOfYear(year, month, day), out timeData);
        }

        public bool TryGetTimeData(DayOfYear day, out SunsetSunriseTime timeData)
        {
            return _sunsetSunriseData.TryGetValue(day, out timeData);
        }

#if UNITY_EDITOR
        [Button]
        public void ImportDataFromCSV()
        {
            if (string.IsNullOrEmpty(_importDataPath)) 
            { 
                Debug.LogError("Import data path is not set."); 
                return; 
            }
            try 
            { 
                _sunsetSunriseData = TimeDataCSVImporter.ImportCSV(_importDataPath); 
                Debug.Log("CSV import completed successfully.");
                UnityEditor.EditorUtility.SetDirty(this);
            } 
            catch (Exception e) 
            { 
                Debug.LogError("Error importing CSV: " + e.Message); 
            }
        }
#endif
    }

    [Serializable]
    public struct SunsetSunriseTime
    {
        public TimeOfDay SunriseTime;
        public TimeOfDay SunsetTime;
    }

    [Serializable]
    public struct DayOfYear
    {
        public short Year;
        public byte Month;
        public byte Day;

        public DayOfYear(int year, int month, int day)
        {
            Year = (short)year;
            Month = (byte)month;
            Day = (byte)day;
        }
    }

    [Serializable]
    public struct TimeOfDay
    {
        public byte Hours;
        public byte Minutes;
    }
}
