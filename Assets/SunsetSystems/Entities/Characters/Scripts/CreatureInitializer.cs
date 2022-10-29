using UnityEngine;
using UMA.CharacterSystem;
using UnityEngine.AI;
using System;

namespace SunsetSystems.Entities.Characters
{
    public static class CreatureInitializer
    {
        public static Creature InitializeCreature(CreatureData data, Vector3 position)
        {
            GameObject creatureObject = UnityEngine.Object.Instantiate(new GameObject());
            creatureObject.name = data.FullName;
            creatureObject.transform.position = position;
            Creature creature = InitializeCreatureScript(creatureObject, data);
            creature.gameObject.layer = LayerMask.NameToLayer("ActionTarget");
            DynamicCharacterAvatar dca = creatureObject.GetComponent<DynamicCharacterAvatar>();
            InitializeUmaAvatar(dca, data);
            StatsManager stats = creatureObject.GetComponent<StatsManager>();
            InitializeStatsManager(stats, data);
            CapsuleCollider collider = creatureObject.GetComponent<CapsuleCollider>();
            InitializeCollider(collider);
            Rigidbody rigidbody = creatureObject.GetComponent<Rigidbody>();
            InitializeRigidbody(rigidbody);
            NavMeshAgent navMeshAgent = creatureObject.GetComponent<NavMeshAgent>();
            InitializeNavMeshAgent(navMeshAgent);
            return creature;
        }

        private static Creature InitializeCreatureScript(GameObject creatureObject, CreatureData data)
        {
            Creature creature = creatureObject.GetComponent<Creature>();
            if (creature == null)
            {
                AddMatchingCreatureScript(creatureObject, data.faction, out creature);
            }
            else if (IsCreatureScriptMismatch(creature, data.faction))
            {
                Debug.LogWarning("Destroying creature script!");
                UnityEngine.Object.DestroyImmediate(creature);
                AddMatchingCreatureScript(creatureObject, data.faction, out creature);
            }
            creature.Data.Inject(data);
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
                Faction.PlayerControlled => !creature.GetType().IsAssignableFrom(typeof(PlayerControlledCharacter)),
                _ => !creature.GetType().IsAssignableFrom(typeof(NPC)),
            };
        }

        private static void InitializeUmaAvatar(DynamicCharacterAvatar dca, CreatureData data)
        {
            dca.loadFileOnStart = true;
            dca.loadFilename = data.umaPresetFileName;
            dca.loadPath = "UMAPresets/";
            dca.loadPathType = DynamicCharacterAvatar.loadPathTypes.Resources;
            dca.raceAnimationControllers.defaultAnimationController = data.animatorControllerAsset;
            dca.DoLoad();

            dca.saveFilename = data.umaPresetFileName;
            dca.savePathType = DynamicCharacterAvatar.savePathTypes.Resources;
            dca.savePath = "UMAPresets/";
        }

        private static void InitializeStatsManager(StatsManager statsManager, CreatureData data)
        {
            statsManager.Initialize(data.stats);
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
