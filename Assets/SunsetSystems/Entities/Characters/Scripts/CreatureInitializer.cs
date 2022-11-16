using UnityEngine;
using UMA.CharacterSystem;
using UnityEngine.AI;
using System;
using Apex.AI.Components;
using System.Threading.Tasks;
using SunsetSystems.Animation;

namespace SunsetSystems.Entities.Characters
{
    public static class CreatureInitializer
    {
        public static Creature InitializeCreature(CreatureData data, Vector3 position)
        {
            return InitializeCreature(data, position, null, Quaternion.identity);
        }

        public static Creature InitializeCreature(CreatureData data, Vector3 position, Transform parent, Quaternion rotation)
        {
            PrepareGameObject(data, position, parent, rotation, out GameObject creatureObject, out Creature creature);
            PrepareComponents(data, creatureObject, creature);
            return creature;

            static void PrepareGameObject(CreatureData data, Vector3 position, Transform parent, Quaternion rotation, out GameObject creatureObject, out Creature creature)
            {
                creatureObject = new();
                creatureObject.transform.parent = parent;
                creatureObject.name = data.FullName;
                creatureObject.transform.SetPositionAndRotation(position, rotation);
                creature = InitializeCreatureScript(creatureObject, data);
                creature.gameObject.layer = LayerMask.NameToLayer("ActionTarget");
            }
        }

        public static void InitializeCreature(Creature creature)
        {
            creature.gameObject.layer = LayerMask.NameToLayer("ActionTarget");
            PrepareComponents(creature.Data, creature.gameObject, creature);
        }

        private static void PrepareComponents(CreatureData data, GameObject creatureObject, Creature owner)
        {
            DynamicCharacterAvatar dca;
            StatsManager stats;
            CapsuleCollider collider;
            Rigidbody rigidbody;
            NavMeshAgent navMeshAgent;
            NavMeshObstacle navMeshObstacle;
            CombatBehaviour combatBehaviour;
            Animator animator;
            CreatureAnimationController animationController;
            UtilityAIComponent utilityAIComponent;

            animator = creatureObject.GetComponent<Animator>() ?? creatureObject.AddComponent<Animator>();
            InitializeAnimator(animator);
            animationController = creatureObject.GetComponent<CreatureAnimationController>() ?? creatureObject.AddComponent<CreatureAnimationController>();
            InitializeAnimationController(animationController);
            dca = creatureObject.GetComponent<DynamicCharacterAvatar>() ?? creatureObject.AddComponent<DynamicCharacterAvatar>();
            InitializeUmaAvatar(dca, data);
            stats = creatureObject.GetComponent<StatsManager>() ?? creatureObject.AddComponent<StatsManager>();
            InitializeStatsManager(stats, owner);
            collider = creatureObject.GetComponent<CapsuleCollider>() ?? creatureObject.AddComponent<CapsuleCollider>();
            InitializeCollider(collider);
            rigidbody = creatureObject.GetComponent<Rigidbody>() ?? creatureObject.AddComponent<Rigidbody>();
            InitializeRigidbody(rigidbody);
            navMeshAgent = creatureObject.GetComponent<NavMeshAgent>() ?? creatureObject.AddComponent<NavMeshAgent>();
            InitializeNavMeshAgent(navMeshAgent);
            navMeshObstacle = creatureObject.GetComponent<NavMeshObstacle>() ?? creatureObject.AddComponent<NavMeshObstacle>();
            InitializeNavMeshObstacle(navMeshObstacle);
            combatBehaviour = creatureObject.GetComponent<CombatBehaviour>() ?? creatureObject.AddComponent<CombatBehaviour>();
            InitializeCombatBehaviour(combatBehaviour);
            utilityAIComponent = creatureObject.GetComponent<UtilityAIComponent>() ?? creatureObject.AddComponent<UtilityAIComponent>();
            InitializeUtilityAI(utilityAIComponent);
        }

        private static Creature InitializeCreatureScript(GameObject creatureObject, CreatureData data)
        {
            Creature creature = creatureObject.GetComponent<Creature>();
            if (creature == null)
            {
                AddMatchingCreatureScript(creatureObject, data.Faction, out creature);
            }
            else if (IsCreatureScriptMismatch(creature, data.Faction))
            {
                Debug.LogWarning("Destroying creature script!");
                UnityEngine.Object.DestroyImmediate(creature);
                AddMatchingCreatureScript(creatureObject, data.Faction, out creature);
            }
            creature.Data = data;
            return creature;
        }

        private static void AddMatchingCreatureScript(GameObject creatureObject, Faction faction, out Creature creature)
        {
            creature = faction switch
            {
                Faction.PlayerControlled => creatureObject.AddComponent<PlayerControlledCharacter>(),
                _ => creatureObject.AddComponent<DefaultNPC>(),
            };
        }

        private static bool IsCreatureScriptMismatch(Creature creature, Faction faction)
        {
            return faction switch
            {
                Faction.PlayerControlled => !creature.GetType().IsAssignableFrom(typeof(PlayerControlledCharacter)),
                _ => !creature.GetType().IsAssignableFrom(typeof(DefaultNPC)),
            };
        }

        private static void InitializeUmaAvatar(DynamicCharacterAvatar dca, CreatureData data)
        {
            dca.loadPathType = DynamicCharacterAvatar.loadPathTypes.Resources;
            dca.loadPath = "UMAPresets/";
            dca.loadFilename = data.UmaPresetFileName;
            dca.loadFileOnStart = true;
            dca.raceAnimationControllers.defaultAnimationController = data.AnimatorControllerAsset;
            dca.saveFilename = data.UmaPresetFileName;
            dca.savePathType = DynamicCharacterAvatar.savePathTypes.Resources;
            dca.savePath = "UMAPresets/";
            dca.DoLoad();
        }

        private static void InitializeStatsManager(StatsManager statsManager, Creature owner)
        {
            statsManager.Initialize(owner);
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

        private static void InitializeNavMeshObstacle(NavMeshObstacle navMeshObstacle)
        {

        }

        private static void InitializeCombatBehaviour(CombatBehaviour combatBehaviour)
        {
            
        }

        private static void InitializeUtilityAI(UtilityAIComponent utilityAIComponent)
        {

        }

        private static void InitializeAnimationController(CreatureAnimationController animationController)
        {

        }

        private static void InitializeAnimator(Animator animator)
        {

        }
    }
}
