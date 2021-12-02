﻿using UnityEngine;
using UMA.CharacterSystem;

namespace Entities.Characters
{
    public class CreatureInitializer
    {
        public static void InitializeCreature(GameObject creatureObject, CreatureAsset asset, Vector3 position)
        {
            creatureObject.transform.position = position;
            Creature creature = InitializeCreatureScript(creatureObject, asset);
            DynamicCharacterAvatar dca = creatureObject.GetComponent<DynamicCharacterAvatar>();
            InitializeUmaAvatar(dca, asset);
            StatsManager stats = creatureObject.GetComponent<StatsManager>();
            InitializeStatsManager(stats, asset);
            CapsuleCollider collider = creatureObject.GetComponent<CapsuleCollider>();
            InitializeCollider(collider);
        }

        private static Creature InitializeCreatureScript(GameObject creatureObject, CreatureAsset asset)
        {
            Creature creature = creatureObject.GetComponent<Creature>();
            if (creature == null)
            {
                if (asset.CreatureFaction.Equals(Faction.Player))
                    creature = creatureObject.AddComponent<Player>();
                else
                    creature = creatureObject.AddComponent<NPC>();
            }
            else if (creature.IsOfType(typeof(NPC)) && asset.CreatureFaction.Equals(Faction.Player) || creature.IsOfType(typeof(Player)) && !asset.CreatureFaction.Equals(Faction.Player))
            {
                Object.DestroyImmediate(creature);
                if (asset.CreatureFaction.Equals(Faction.Player))
                    creature = creatureObject.AddComponent<Player>();
                else
                    creature = creatureObject.AddComponent<NPC>();
            }
            return creature;
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

        private static void InitializeCollider(CapsuleCollider collider)
        {
            collider.height = 1.8f;
            collider.center = new Vector3(0, .9f, 0);
            collider.radius = 0.35f;
        }
    }
}
