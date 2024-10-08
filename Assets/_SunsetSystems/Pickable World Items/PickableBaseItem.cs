using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Core.Localization;
using SunsetSystems.Core.Rendering;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interactable;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Utils.ObjectPooling;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.PickableItems
{
    public class PickableBaseItem : SerializedMonoBehaviour, IInteractionHandler, IPooledObject
    {
        public static Action<PickableBaseItem, IBaseItem> OnItemPickup;

        [Title("Config")]
        [SerializeField, MinValue(1)]
        private int _stackSize = 1;
        [SerializeField]
        private IBaseItem _itemReference;
        [Title("References")]
        [SerializeField]
        private ILocalizationTarget _pickableNameplate;
        [SerializeField]
        private IRendererProcessor _highlightHandler;
        [SerializeField]
        private GameObject _pickableMeshInstance;

        public bool HandleInteraction(IActionPerformer interactee)
        {
            bool result = false;
            if (_itemReference != null)
            {
                if (interactee.References.CreatureData.Faction == Faction.PlayerControlled)
                {
                    result = DoPickupByPlayer(interactee);
                }
                else
                {
                    Debug.LogError("Item pickup failed! Item pickup by non-player entities is not supported!");
                }
            }
            return result;
        }

        private bool DoPickupByPlayer(IActionPerformer interactee)
        {
            bool result = InventoryManager.Instance.GiveItemToPlayer(_itemReference, _stackSize);
            if (result)
            {
                OnItemPickup?.Invoke(this, _itemReference);
            }
            else
            {
                Debug.LogError($"Item pickup by player failed! Item: {gameObject.name}; Pickup attempted by: {interactee.References.GameObject.name}");
            }
            return result;
        }

        public async void SetupPickable(IBaseItem item)
        {
            if (_itemReference != null)
                ResetObject();
            _itemReference = item;
            var loadingOp = Addressables.InstantiateAsync(item.WorldSpaceRepresentation, transform);
            await loadingOp.Task;
            _pickableMeshInstance = loadingOp.Result;
            var renderers = _pickableMeshInstance.GetComponentsInChildren<Renderer>(true);
            if (renderers != null && renderers.Length > 0)
                _highlightHandler.InjectRenderers(renderers);
            SetupMeshColliders(renderers);
            _pickableNameplate.SetLocalizedText(item.Name);
            gameObject.name = item.Name;
        }

#if UNITY_EDITOR
        [Button]
        public void SetupPickableEditor(IBaseItem item)
        {
            if (UnityEditor.EditorApplication.isPlaying is false)
            {
                if (_itemReference != null)
                    ResetObjectEditor();
                _itemReference = item;
                var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(item.WorldSpaceRepresentation.AssetGUID);
                var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                _pickableMeshInstance = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
                var renderers = _pickableMeshInstance.GetComponentsInChildren<Renderer>(true);
                if (renderers != null && renderers.Length > 0)
                    _highlightHandler.InjectRenderers(renderers);
                SetupMeshColliders(renderers);
                _pickableNameplate.SetLocalizedText(item.Name);
                UnityEditor.EditorUtility.SetDirty(this);
                UnityEditor.EditorUtility.SetDirty(_highlightHandler as UnityEngine.Object);
                UnityEditor.EditorUtility.SetDirty(_pickableNameplate as UnityEngine.Object);
            }
            else
            {
                SetupPickable(item);
            }
        }

        private void ResetObjectEditor()
        {
            _itemReference = null;
            if (_pickableMeshInstance)
                DestroyImmediate(_pickableMeshInstance);
            _pickableMeshInstance = null;
            _highlightHandler.InjectRenderers(null);
            _pickableNameplate.SetLocalizedText("");
        }
#endif

        private void SetupMeshColliders(IEnumerable<Renderer> renderers)
        {
            var existingColliders = _pickableMeshInstance.GetComponentsInChildren<Collider>();
            if (existingColliders != null && existingColliders.Length > 0)
                return;
            foreach (var renderer in renderers)
            {
                if (renderer.TryGetComponent(out Collider _))
                    return;
                if (renderer.TryGetComponent(out MeshFilter meshFilter))
                {
                    var meshCollider = renderer.gameObject.AddComponent<MeshCollider>();
                    meshCollider.convex = true;
                    meshCollider.sharedMesh = meshFilter.sharedMesh;
                }
            }
        }

        public void ResetObject()
        {
            _itemReference = null;
            if (_pickableMeshInstance)
                Addressables.ReleaseInstance(_pickableMeshInstance);
            _pickableMeshInstance = null;
            _highlightHandler.InjectRenderers(null);
            _pickableNameplate.SetLocalizedText("");
        }
    }
}
