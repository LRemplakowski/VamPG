using UnityEngine;
using UMA.CharacterSystem;
using UnityEngine.AI;
using System;

namespace Entities.Characters
{
    public static class CreatureInitializer
    {
        public static Creature InitializeCreature(GameObject creatureObject, CreatureAsset asset, Vector3 position)
        {
            creatureObject.transform.position = position;
            Creature creature = InitializeCreatureScript(creatureObject, asset);
            DynamicCharacterAvatar dca = creatureObject.GetComponent<DynamicCharacterAvatar>();
            InitializeUmaAvatar(dca, asset);
            StatsManager stats = creatureObject.GetComponent<StatsManager>();
            InitializeStatsManager(stats, asset);
            CapsuleCollider collider = creatureObject.GetComponent<CapsuleCollider>();
            InitializeCollider(collider);
            Rigidbody rigidbody = creatureObject.GetComponent<Rigidbody>();
            InitializeRigidbody(rigidbody);
            NavMeshAgent navMeshAgent = creatureObject.GetComponent<NavMeshAgent>();
            InitializeNavMeshAgent(navMeshAgent);
            return creature;
        }

        private static Creature InitializeCreatureScript(GameObject creatureObject, CreatureAsset asset)
        {
            Creature creature = creatureObject.GetComponent<Creature>();
            if (creature == null)
            {
                AddMatchingCreatureScript(creatureObject, asset.CreatureFaction, out creature);
            }
            else if (IsCreatureScriptMismatch(creature, asset.CreatureFaction))
            {
                UnityEngine.Object.DestroyImmediate(creature);
                AddMatchingCreatureScript(creatureObject, asset.CreatureFaction, out creature);
            }
            return creature;
        }

        private static void AddMatchingCreatureScript(GameObject creatureObject, Faction faction, out Creature creature)
        {
            creature = faction switch
            {
                Faction.PlayerControlled => creatureObject.AddComponent<PlayerControlledCharacter>(),
                _ => creatureObject.AddComponent<NPC>(),
            };
        }

        private static bool IsCreatureScriptMismatch(Creature creature, Faction faction)
        {
            return faction switch
            {
                Faction.PlayerControlled => creature.IsOfType(typeof(PlayerControlledCharacter)),
                _ => creature.IsOfType(typeof(NPC)),
            };
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

        private static void InitializeRigidbody(Rigidbody rigidbody)
        {
            rigidbody.isKinematic = true;
        }

        private static void InitializeNavMeshAgent(NavMeshAgent navMeshAgent)
        {
            navMeshAgent.speed = 4.0f;
            navMeshAgent.angularSpeed = 360.0f;
            navMeshAgent.acceleration = 8.0f;
        }
    }
}
