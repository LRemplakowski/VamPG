using System.Collections;
using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Utils.Database;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Core.UMA
{
    public interface IUMAManager
    {
        UMAWardrobeCollection BaseLookWardrobeCollection { get; }

        void BuildUMAFromTemplate(ICreatureTemplate template);
    }

    public class UMAManager : SerializedMonoBehaviour, IUMAManager
    {
        [SerializeField, Required]
        private ScriptableUMAConfig _umaConfig;
        [SerializeField, Required]
        private GameObject _umaRoot;
        [field: SerializeField]
        public UMAWardrobeCollection BaseLookWardrobeCollection { get; private set; }
        [SerializeField, ReadOnly]
        private DynamicCharacterAvatar _umaAvatar;

        private IEnumerator _updateOnNextFrame;

        private void Start()
        {
            if (_umaAvatar == null)
                PrepareUMA();
            LoadDefaultWardrobeCollection(BaseLookWardrobeCollection);
            _umaAvatar.BuildCharacter(true);
        }

        private IEnumerator RebuildUmaOnNextFrame()
        {
            yield return null;
            _umaAvatar.BuildCharacter(true);
            _updateOnNextFrame = null;
        }

        public async void BuildUMAFromTemplate(ICreatureTemplate template)
        {
            if (_umaAvatar == null)
                PrepareUMA();
#if !UNITY_EDITOR
            await new WaitForUpdate();
            bool umaFinished = false;
            _umaAvatar.CharacterCreated.AddListener(OnUMADone);
            _umaAvatar.CharacterUpdated.AddListener(OnUMADone);
            await new WaitUntil(() => umaFinished);
            _umaAvatar.CharacterCreated.RemoveListener(OnUMADone);
            _umaAvatar.CharacterUpdated.RemoveListener(OnUMADone);
            void OnUMADone(UMAData data)
            {
                umaFinished = true;
            }
#endif
            SetBodyType(template.BodyType);
            if (DatabaseHolder.Instance != null)
            {
                var wardrobeDB = DatabaseHolder.Instance.GetDatabase<WardrobeCollectionDatabaseFile>();
                if (wardrobeDB != null)
                    LoadDefaultWardrobeCollection(wardrobeDB.GetAsset(template.BaseLookWardrobeCollectionID)?.Asset);
            }
            else
            {
                LoadDefaultWardrobeCollection(template.BaseLookWardrobeCollectionAsset);
            }
#if !UNITY_EDITOR
            await new WaitForUpdate();
#endif
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
            _umaAvatar = _umaRoot.GetComponent<DynamicCharacterAvatar>();
            if (_umaAvatar == null)
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
            if (BaseLookWardrobeCollection != null)
                _umaAvatar.LoadWardrobeCollection(BaseLookWardrobeCollection);
        }

        private void SetBodyType(BodyType bodyType)
        {
            _umaAvatar.ChangeRace(_umaConfig.BodyRaceData[bodyType], DynamicCharacterAvatar.ChangeRaceOptions.useDefaults, true);
        }

        private void LoadDefaultWardrobeCollection(UMAWardrobeCollection defaultWardrobeCollection)
        {
            if (BaseLookWardrobeCollection != null)
                _umaAvatar.UnloadWardrobeCollection(BaseLookWardrobeCollection.name);
            BaseLookWardrobeCollection = defaultWardrobeCollection;
            if (BaseLookWardrobeCollection == null)
                return;
            _umaAvatar.LoadWardrobeCollection(BaseLookWardrobeCollection);
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
