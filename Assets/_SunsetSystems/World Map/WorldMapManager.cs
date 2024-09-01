using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SunsetSystems.Core.SceneLoading;
using SunsetSystems.Persistence;
using UnityEngine;

namespace SunsetSystems.WorldMap
{
    public class WorldMapManager : SerializedMonoBehaviour, IWorldMapManager, ISaveable
    {
        [Title("References")]
        [SerializeField]
        private List<IWorldMapData> _defaultUnlockedMaps = new();

        [Title("Runtime")]
        [ShowInInspector]
        private HashSet<IWorldMapData> _unlockedMaps = new();

        public string DataKey => DataKeyConstants.WORLD_MAP_MANAGER_DATA_KEY;

        private void Awake()
        {
            ISaveable.RegisterSaveable(this);
            _unlockedMaps.AddRange(_defaultUnlockedMaps);
        }

        private void OnDestroy()
        {
            ISaveable.UnregisterSaveable(this);
        }

        public IEnumerable<IWorldMapData> GetUnlockedMaps()
        {
            return _unlockedMaps.AsEnumerable();
        }

        public bool IsMapUnlocked(IWorldMapData mapData)
        {
            return _unlockedMaps.Contains(mapData) || _unlockedMaps.Any(map => map.DatabaseID == mapData.DatabaseID);
        }

        public void SetMapUnlocked(IWorldMapData mapData, bool unlocked)
        {
            if (mapData == null)
            {
                Debug.LogError("World Map Manager was instructed to unlock NULL map! Aborting...");
                return;
            }

            if (unlocked)
                _unlockedMaps.Add(mapData);
            else
                _unlockedMaps.Remove(mapData);
        }

        public void TravelToMap(IWorldMapData mapData)
        {
            _ = LevelLoader.Instance.LoadNewScene(mapData.LevelData);
        }

        public object GetSaveData()
        {
            return new WorldMapSaveData(this);
        }

        public void InjectSaveData(object data)
        {
            if (data is not WorldMapSaveData mapData)
                return;
            _unlockedMaps ??= new();
            foreach (var mapID in mapData.UnlockedMapsIDs)
            {
                if (WorldMapEntryDatabase.Instance.TryGetEntry(mapID, out IWorldMapData entry))
                    _unlockedMaps.Add(entry);
            }
        }

        public class WorldMapSaveData : SaveData
        {
            public List<string> UnlockedMapsIDs;

            public WorldMapSaveData(IWorldMapManager manager) : base()
            {
                UnlockedMapsIDs = manager.GetUnlockedMaps().Select(map => map.DatabaseID).ToList();
            }
        }
    }
}
