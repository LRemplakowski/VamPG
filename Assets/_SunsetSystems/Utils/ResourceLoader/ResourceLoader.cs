using SunsetSystems.Entities.Characters;
using UnityEngine;
using UnityEngine.AI;

namespace SunsetSystems.Resources
{
    public static class ResourceLoader
    {
        // Main character assets
        private const string CHARACTER_MALE_AGENT_PATH = "CharacterPresets/MaleAgent";
        private const string CHARACTER_MALE_CONVICT_PATH = "CharacterPresets/MaleConvict";
        private const string CHARACTER_MALE_JOURNALIST_PATH = "CharacterPresets/MaleJournalist";
        private const string CHARACTER_FEMALE_AGENT_PATH = "CharacterPresets/FemaleAgent";
        private const string CHARACTER_FEMALE_CONVICT_PATH = "CharacterPresets/FemaleConvict";
        private const string CHARACTER_FEMALE_JOURNALIST_PATH = "CharacterPresets/FemaleJournalist";
        // Character debug fallback
        private const string CHARACTER_DEBUG = "DEBUG/default";
        private const string EMPTY_CREATURE_PREFAB = "DEBUG/CreatureData";
        private const string FALLBACK_ICON_PATH = "DEBUG/missing";

        private const string ANIMATOR_CONTROLLERS_PATH = "Animation/AnimationControllers/";

        // Combat
        private const string GRID_HELPER_AGENT_PATH = "Combat/GridNavAgent";
        private const string TARGETING_LINE_PATH = "Combat/TargetingLine";

        private const string HOVER_HIGHLIGHT_MATERIAL = "hover_highlight_material";
        private const string PORTRAIT_DIRECTORY = "CharacterPortraits/";

        private const string AUDIO_SFX = "SFX/";
        private const string AUDIO_SOUNDTRACK = "Soundtrack/";

        private static T GetAsset<T>(string path) where T : Object
        {
            return UnityEngine.Resources.Load<T>(path);
        }

        public static CreatureConfig GetMaleAgentAsset()
        {
            return GetAsset<CreatureConfig>(CHARACTER_MALE_AGENT_PATH);
        }

        public static CreatureConfig GetMaleConvictAsset()
        {
            return GetAsset<CreatureConfig>(CHARACTER_MALE_CONVICT_PATH);
        }

        public static CreatureConfig GetMaleJournalistAsset()
        {
            return GetAsset<CreatureConfig>(CHARACTER_MALE_JOURNALIST_PATH);
        }

        public static CreatureConfig GetFemaleAgentAsset()
        {
            return GetAsset<CreatureConfig>(CHARACTER_FEMALE_AGENT_PATH);
        }

        public static CreatureConfig GetFemaleConvictAsset()
        {
            return GetAsset<CreatureConfig>(CHARACTER_FEMALE_CONVICT_PATH);
        }

        public static CreatureConfig GetFemaleJournalistAsset()
        {
            return GetAsset<CreatureConfig>(CHARACTER_FEMALE_JOURNALIST_PATH);
        }

        public static CreatureConfig GetDefaultCreatureAsset()
        {
            return GetAsset<CreatureConfig>(CHARACTER_DEBUG);
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

        public static NavMeshAgent GetGridHelperAgentPrefab()
        {
            return GetAsset<NavMeshAgent>(GRID_HELPER_AGENT_PATH);
        }

        public static LineRenderer GetTargetingLineRendererPrefab()
        {
            return GetAsset<LineRenderer>(TARGETING_LINE_PATH);
        }

        public static Material GetHoverHighlightMaterial()
        {
            return GetAsset<Material>(HOVER_HIGHLIGHT_MATERIAL);
        }

        public static Sprite GetPortrait(string portraitName)
        {
            return GetAsset<Sprite>($"{PORTRAIT_DIRECTORY}{portraitName}");
        }

        public static AudioClip GetSFX(string sfxName)
        {
            return GetAsset<AudioClip>($"{AUDIO_SFX}{sfxName}");
        }

        public static AudioClip GetSong(string songName)
        {
            return GetAsset<AudioClip>($"{AUDIO_SOUNDTRACK}{songName}");
        }
    }
}