using System.Collections;
using System.Linq;
using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory.Data;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.UMA
{
    public interface IUMAManager
    {
        string BaseLookWardrobeReadableID { get; }

        void BuildUMAFromTemplate(ICreatureTemplate template);
    }

    public class UMAManager : SerializedMonoBehaviour, IUMAManager
    {
        [SerializeField, Required]
        private ScriptableUMAConfig _umaConfig;
        [SerializeField, Required]
        private GameObject _umaRoot;
        [field: SerializeField, ReadOnly]
        public string BaseLookWardrobeReadableID { get; private set; }
        [ShowInInspector, ReadOnly]
        private UMAWardrobeCollection _baseLookWardrobeCollection;
        [SerializeField, ReadOnly]
        private DynamicCharacterAvatar _umaAvatar;

        private IEnumerator _updateOnNextFrame;

        private void Start()
        {
            if (_umaAvatar == null)
                PrepareUMA();
            LoadDefaultWardrobeCollection(BaseLookWardrobeReadableID);
            _umaAvatar.BuildCharacter(true);
        }

        private UMAWardrobeCollection WardrobeCollectionFromID(string readableID)
        {
            if (string.IsNullOrWhiteSpace(readableID))
                return null;
            if (UMAWardrobeDatabase.Instance.TryGetEntryByReadableID(readableID, out var entry))
                return entry.Data;
            return null;
        }

        private IEnumerator RebuildUmaOnNextFrame()
        {
            yield return null;
            _umaAvatar.BuildCharacter(true);
            _updateOnNextFrame = null;
        }

        public void BuildUMAFromTemplate(ICreatureTemplate template)
        {
            if (_umaAvatar == null)
                PrepareUMA();
            SetBodyType(template.BodyType);
            BaseLookWardrobeReadableID = template.BaseLookWardrobeReadableID;
            LoadDefaultWardrobeCollection(BaseLookWardrobeReadableID);
            _umaAvatar.BuildCharacter();
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode is false)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }

        [Button]
        private void PrepareUMA()
        {
            if (!_umaRoot.TryGetComponent(out _umaAvatar))
            {
                _umaAvatar = _umaRoot.AddComponent<DynamicCharacterAvatar>();
#if UNITY_EDITOR
                _umaAvatar.editorTimeGeneration = false;
#endif
                _umaAvatar.RacePreset = _umaConfig.BodyRaceData[BodyType.Female].raceName;
                _umaAvatar.RecreateAnimatorOnRaceChange = false;
                _umaAvatar.KeepAnimatorController = true;
                _umaAvatar.predefinedDNA = new();
                DynamicCharacterAvatar.RaceAnimator maleAnimator = new()
                {
                    raceName = _umaConfig.BodyRaceData[BodyType.Male].raceName,
                    animatorController = _umaConfig.RaceAnimators[_umaConfig.BodyRaceData[BodyType.Male]],
                    animatorControllerName = _umaConfig.RaceAnimators[_umaConfig.BodyRaceData[BodyType.Male]].name
                };
                DynamicCharacterAvatar.RaceAnimator femaleAnimator = new()
                {
                    raceName = _umaConfig.BodyRaceData[BodyType.Female].raceName,
                    animatorController = _umaConfig.RaceAnimators[_umaConfig.BodyRaceData[BodyType.Female]],
                    animatorControllerName = _umaConfig.RaceAnimators[_umaConfig.BodyRaceData[BodyType.Female]].name
                };
                _umaAvatar.raceAnimationControllers.animators.Add(maleAnimator);
                _umaAvatar.raceAnimationControllers.animators.Add(femaleAnimator);
            }
            _umaAvatar.WardrobeRecipes.Clear();
            DoLoadBaseLook(_baseLookWardrobeCollection);
        }

        private void SetBodyType(BodyType bodyType)
        {
            _umaAvatar.ChangeRace(_umaConfig.BodyRaceData[bodyType], DynamicCharacterAvatar.ChangeRaceOptions.useDefaults, true);
        }

        private void LoadDefaultWardrobeCollection(string wardrobeID)
        {
            if (_baseLookWardrobeCollection != null)
                _umaAvatar.UnloadWardrobeCollection(_baseLookWardrobeCollection.name);
            _baseLookWardrobeCollection = WardrobeCollectionFromID(wardrobeID);
            DoLoadBaseLook(_baseLookWardrobeCollection);
        }

        private void DoLoadBaseLook(UMAWardrobeCollection baseLookCollection)
        {
            if (baseLookCollection != null)
            {
                _umaAvatar.LoadWardrobeCollection(baseLookCollection);
                var skinColor = baseLookCollection.SharedColors.First(sc => sc.name == "Skin");
                if (skinColor)
                    _umaAvatar.SetColor(skinColor.name, skinColor);
            }
        }

        public async void OnItemEquipped(IEquipableItem item)
        {
            if (item is IWearable wearable && _umaAvatar != null)
            {
                if (wearable.WearableWardrobe != null)
                {
                    _umaAvatar.LoadWardrobeCollection(wearable.WearableWardrobe);
                    if (_umaAvatar.umaData == null)
                        await new WaitUntil(() => _umaAvatar.umaData != null);
                    _umaAvatar.umaData.Dirty();
                    if (_updateOnNextFrame == null)
                    {
                        _updateOnNextFrame = RebuildUmaOnNextFrame();
                        _ = StartCoroutine(_updateOnNextFrame);
                    }
                }
                else
                {
                    Debug.LogWarning($"Item {wearable} of Creature {_umaRoot.transform.parent.name} has a null WardrobeCollection reference!");
                }
            }
        }

        public async void OnItemUnequipped(IEquipableItem item)
        {
            if (item is IWearable wearable)
            {
                if (wearable.WearableWardrobe != null)
                {
                    _umaAvatar.UnloadWardrobeCollection(wearable.WearableWardrobe.name);
                    if (_umaAvatar.umaData == null)
                        await new WaitUntil(() => _umaAvatar.umaData != null);
                    _umaAvatar.umaData.Dirty();
                    if (_updateOnNextFrame == null)
                    {
                        _updateOnNextFrame = RebuildUmaOnNextFrame();
                        _ = StartCoroutine(_updateOnNextFrame);
                    }
                }
            }
        }
    }
}
