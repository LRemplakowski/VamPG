using Sirenix.OdinInspector;
using Sirenix.Utilities;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMA;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Core.UMA
{
    public interface IUMAManager
    {
        void PrepareUMA();
        void InjectDefaultRecipes(List<UMARecipeBase> defaultRecipes);
    }

    public class UMAManager : SerializedMonoBehaviour, IUMAManager
    {
        [SerializeField, Required]
        private ScriptableUMAConfig umaConfig;
        [SerializeField, Required]
        private GameObject umaRoot;
        [SerializeField]
        private HashSet<UMARecipeBase> defaultRecipes = new();
        [SerializeField, ReadOnly]
        private DynamicCharacterAvatar umaAvatar;

        [Button]
        public void PrepareUMA()
        {
            if (umaRoot.TryGetComponent(out UMAData umaData) is false)
            {
                umaData = umaRoot.AddComponent<UMAData>();
            }
            umaAvatar = umaRoot.GetComponent<DynamicCharacterAvatar>();
            if (umaAvatar == null)
            {
                umaAvatar = umaRoot.AddComponent<DynamicCharacterAvatar>();
                umaAvatar.umaData = umaData;
                umaAvatar.predefinedDNA = new();
                umaAvatar.ChangeRace(umaConfig.BodyRaceData[BodyType.Male].raceName, true);
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
        }

        public void InjectDefaultRecipes(List<UMARecipeBase> defaultRecipes)
        {
            this.defaultRecipes.AddRange(defaultRecipes);
            umaAvatar.WardrobeRecipes.Clear();
            umaAvatar.AddAdditionalSerializedRecipes(this.defaultRecipes.ToList());
        }

        public void OnItemEquipped(IEquipableItem item)
        {
            if (item is IWearable wearable)
            {
                umaAvatar.LoadWardrobeCollection(wearable.WearableWardrobe);
            }
        }

        public void OnItemUnequipped(IEquipableItem item)
        {
            if (item is IWearable wearable)
            {
                umaAvatar.UnloadWardrobeCollectionGroup(wearable.WearableWardrobe.wardrobeSlot);
            }
        }
    }
}
