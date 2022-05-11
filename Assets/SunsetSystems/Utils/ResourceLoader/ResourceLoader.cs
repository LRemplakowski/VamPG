using Entities.Characters;
using UnityEngine;

namespace SunsetSystems.Resources
{
    public static class ResourceLoader
    {
        /// <summary>
        /// Main character assets
        /// </summary>
        private const string CHARACTER_MALE_AGENT_PATH = "CharacterPresets/MaleAgent";
        private const string CHARACTER_MALE_CONVICT_PATH = "CharacterPresets/MaleConvict";
        private const string CHARACTER_MALE_JOURNALIST_PATH = "CharacterPresets/MaleJournalist";
        private const string CHARACTER_FEMALE_AGENT_PATH = "CharacterPresets/FemaleAgent";
        private const string CHARACTER_FEMALE_CONVICT_PATH = "CharacterPresets/FemaleConvict";
        private const string CHARACTER_FEMALE_JOURNALIST_PATH = "CharacterPresets/FemaleJournalist";
        /// <summary>
        /// Character debug fallback
        /// </summary>
        private const string CHARACTER_DEBUG = "DEBUG/default";
        private const string EMPTY_CREATURE_PREFAB = "DEBUG/CreatureData";
        private const string FALLBACK_ICON_PATH = "DEBUG/missing";

        private const string ANIMATOR_CONTROLLERS_PATH = "Animation/AnimationControllers/";

        private static T GetAsset<T>(string path) where T : Object
        {
            return UnityEngine.Resources.Load<T>(path);
        }

        public static CreatureAsset GetMaleAgentAsset()
        {
            return GetAsset<CreatureAsset>(CHARACTER_MALE_AGENT_PATH);
        }

        public static CreatureAsset GetMaleConvictAsset()
        {
            return GetAsset<CreatureAsset>(CHARACTER_MALE_CONVICT_PATH);
        }

        public static CreatureAsset GetMaleJournalistAsset()
        {
            return GetAsset<CreatureAsset>(CHARACTER_MALE_JOURNALIST_PATH);
        }

        public static CreatureAsset GetFemaleAgentAsset()
        {
            return GetAsset<CreatureAsset>(CHARACTER_FEMALE_AGENT_PATH);
        }

        public static CreatureAsset GetFemaleConvictAsset()
        {
            return GetAsset<CreatureAsset>(CHARACTER_FEMALE_CONVICT_PATH);
        }

        public static CreatureAsset GetFemaleJournalistAsset()
        {
            return GetAsset<CreatureAsset>(CHARACTER_FEMALE_JOURNALIST_PATH);
        }

        public static CreatureAsset GetDefaultCreatureAsset()
        {
            return GetAsset<CreatureAsset>(CHARACTER_DEBUG);
        }

        public static CreatureData GetEmptyCreaturePrefab()
        {
            return GetAsset<CreatureData>(EMPTY_CREATURE_PREFAB);
        }

        public static RuntimeAnimatorController GetAnimatorController(string resourceName)
        {
            return GetAsset<RuntimeAnimatorController>(ANIMATOR_CONTROLLERS_PATH + resourceName);
        }

        public static RuntimeAnimatorController GetFallbackAnimator()
        {
            return GetAsset<RuntimeAnimatorController>(ANIMATOR_CONTROLLERS_PATH + "female_anims");
        }

        public static Sprite GetFallbackIcon()
        {
            return GetAsset<Sprite>(FALLBACK_ICON_PATH);
        }
    }
}