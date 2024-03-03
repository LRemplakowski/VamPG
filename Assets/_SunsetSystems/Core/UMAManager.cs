using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Inventory.Data;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Core.UMA
{
    public interface IUMAManager
    {
        void BuildUMAFromTemplate(ICreatureTemplate template);
    }

    public class UMAManager : SerializedMonoBehaviour, IUMAManager
    {
        [SerializeField, Required]
        private ScriptableUMAConfig umaConfig;
        [SerializeField, Required]
        private GameObject umaRoot;
        [SerializeField]
        private List<UMARecipeBase> defaultRecipes = new();
        [SerializeField, ReadOnly]
        private DynamicCharacterAvatar umaAvatar;

        private void Start()
        {
            if (umaAvatar == null)
                PrepareUMA();
        }

        public void BuildUMAFromTemplate(ICreatureTemplate template)
        {
            if (umaAvatar == null)
                PrepareUMA();
            SetBodyType(template.BodyType);
            InjectDefaultRecipes(template.BaseUmaRecipes);
            umaAvatar.UpdatePending();
            umaAvatar.BuildCharacter();
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
            umaAvatar = umaRoot.GetComponent<DynamicCharacterAvatar>();
            if (umaAvatar == null)
            {
                umaAvatar = umaRoot.AddComponent<DynamicCharacterAvatar>();
                umaAvatar.RacePreset = umaConfig.BodyRaceData[BodyType.Female].raceName;
                umaAvatar.RecreateAnimatorOnRaceChange = false;
                umaAvatar.predefinedDNA = new();
                DynamicCharacterAvatar.RaceAnimator maleAnimator = new()
                {
                    raceName = umaConfig.BodyRaceData[BodyType.Male].raceName,
                    animatorController = umaConfig.RaceAnimators[umaConfig.BodyRaceData[BodyType.Male]],
                    animatorControllerName = umaConfig.RaceAnimators[umaConfig.BodyRaceData[BodyType.Male]].name
                };
                DynamicCharacterAvatar.RaceAnimator femaleAnimator = new()
                {
                    raceName = umaConfig.BodyRaceData[BodyType.Female].raceName,
                    animatorController = umaConfig.RaceAnimators[umaConfig.BodyRaceData[BodyType.Female]],
                    animatorControllerName = umaConfig.RaceAnimators[umaConfig.BodyRaceData[BodyType.Female]].name
                };
                umaAvatar.raceAnimationControllers.animators.Add(maleAnimator);
                umaAvatar.raceAnimationControllers.animators.Add(femaleAnimator);
            }
            umaAvatar.WardrobeRecipes.Clear();
            umaAvatar.AddAdditionalSerializedRecipes(this.defaultRecipes.Distinct().ToList());
            umaAvatar.UpdatePending();
            umaAvatar.BuildCharacter(true);
        }

        private void SetBodyType(BodyType bodyType)
        {
            umaAvatar.ChangeRace(umaConfig.BodyRaceData[bodyType], DynamicCharacterAvatar.ChangeRaceOptions.useDefaults, true);
        }

        private void InjectDefaultRecipes(List<UMARecipeBase> defaultRecipes)
        {
            if (defaultRecipes == null || defaultRecipes.Count <= 0)
                return;
            this.defaultRecipes.AddRange(defaultRecipes);
            umaAvatar.WardrobeRecipes.Clear();
            umaAvatar.AddAdditionalSerializedRecipes(this.defaultRecipes.Distinct().ToList());
        }

        public void OnItemEquipped(IEquipableItem item)
        {
            if (item is IWearable wearable)
            {
                umaAvatar.LoadWardrobeCollection(wearable.WearableWardrobe);
                umaAvatar.UpdatePending();
                umaAvatar.BuildCharacter(true);
            }
        }

        public void OnItemUnequipped(IEquipableItem item)
        {
            if (item is IWearable wearable)
            {
                umaAvatar.UnloadWardrobeCollection(wearable.WearableWardrobe.name);
                umaAvatar.UpdatePending();
                umaAvatar.BuildCharacter(true);
            }
        }
    }
}
