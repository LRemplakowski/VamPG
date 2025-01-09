using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Persistence;
using SunsetSystems.Utils.ObjectPooling;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.PickableItems
{
    public class PickableItemsManager : SerializedMonoBehaviour, ISaveable
    {
        public PickableItemsManager Instance { get; private set; }

        [Title("Config")]
        [SerializeField]
        private IObjectPool<PickableBaseItem> _pickablePrefab;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private Dictionary<PickableBaseItem, IBaseItem> _dynamicPickables = new();

        public string DataKey => DataKeyConstants.PICKABLES_MANAGER_DATA_KEY;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                ISaveable.RegisterSaveable(this);
                PickableBaseItem.OnItemPickup += OnItemPickup;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            PickableBaseItem.OnItemPickup -= OnItemPickup;
            ISaveable.UnregisterSaveable(this);
        }

        public void ClearAllPickables()
        {
            foreach (var pickable in _dynamicPickables)
            {
                Addressables.ReleaseInstance(pickable.Key.gameObject);
            }
            _dynamicPickables.Clear();
        }

        private void OnItemPickup(PickableBaseItem pickable, IBaseItem item)
        {
            if (_dynamicPickables.ContainsKey(pickable))
            {
                _dynamicPickables.Remove(pickable);
                Addressables.ReleaseInstance(pickable.gameObject);
            }
            else
            {
                pickable.gameObject.SetActive(false);
            }
        }

        public void DropItem(IBaseItem item, Vector3 worldPosition)
        {
            DropItem(item, worldPosition, Quaternion.identity);
        }

        public void DropItem(IBaseItem item, Vector3 worldPosition, Quaternion rotation)
        {
            var pickable = _pickablePrefab.GetPooledObject(worldPosition, rotation);
            pickable.SetupPickable(item);
            _dynamicPickables.Add(pickable, item);
            //var loadingOp = Addressables.InstantiateAsync(item.WorldSpaceRepresentation);
            //var instance = await loadingOp.Task;
            //_dynamicPickables.Add(instance, item);
        }

        public object GetSaveData()
        {
            return new PickableItemsSaveData(this);
        }

        public bool InjectSaveData(object data)
        {
            if (data is not PickableItemsSaveData saveData)
                return false;
            if (saveData.PickableItems == null)
                return false;
            foreach (var itemData in saveData.PickableItems)
            {
                if (ItemDatabase.Instance.TryGetEntry(itemData.ItemID, out IBaseItem item))
                {
                    DropItem(item, itemData.WorldPosition, itemData.Rotation);
                }
            }
            return true;
        }

        [Serializable]
        public class PickableItemsSaveData : SaveData
        {
            public List<PickableItemData> PickableItems = new();

            public PickableItemsSaveData(PickableItemsManager pickablesManager) : base()
            {
                foreach (var pickableItemPair in pickablesManager._dynamicPickables)
                {
                    var pickable = pickableItemPair.Key;
                    var item = pickableItemPair.Value;
                    PickableItems.Add(new()
                    {
                        WorldPosition = pickable.transform.position,
                        Rotation = pickable.transform.rotation,
                        ItemID = item.DatabaseID,
                    });
                }
            }

            public PickableItemsSaveData() : base()
            {
                PickableItems = new();
            }
        }

        [Serializable]
        public struct PickableItemData
        {
            public Vector3 WorldPosition;
            public Quaternion Rotation;
            public string ItemID;
        }
    }
}
