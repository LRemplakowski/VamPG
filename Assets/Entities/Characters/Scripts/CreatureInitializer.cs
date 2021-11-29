using UnityEngine;
using UMA.CharacterSystem;

namespace Entities.Characters
{
    public class CreatureInitializer
    {
        public static void InitializeCreature(GameObject creatureObject, CreatureAsset asset, Vector3 position)
        {
            Creature creature = creatureObject.AddComponent<Player>();
            DynamicCharacterAvatar dca = creatureObject.GetComponent<DynamicCharacterAvatar>();
            InitializeUmaAvatar(dca, asset);
            StatsManager stats = creatureObject.GetComponent<StatsManager>();
            InitializeStatsManager(stats, asset);
        }

        private static void InitializeUmaAvatar(DynamicCharacterAvatar dca, CreatureAsset asset)
        {
            dca.loadFileOnStart = true;
            dca.loadFilename = asset.UmaPresetFilename;
            dca.loadPath = "UMAPresets";
            dca.loadPathType = DynamicCharacterAvatar.loadPathTypes.Resources;
            dca.raceAnimationControllers.defaultAnimationController = asset.AnimatorController;
            dca.DoLoad();

            dca.saveFilename = asset.UmaPresetFilename;
            dca.savePathType = DynamicCharacterAvatar.savePathTypes.Resources;
            dca.savePath = "UMAPresets";
        }

        private static void InitializeStatsManager(StatsManager statsManager, CreatureAsset asset)
        {
            statsManager.Stats = asset.StatsAsset;
        }
    }
}
