using SunsetSystems.Entities.Characters;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UMA;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Inventory;

namespace SunsetSystems.Animation
{
    public class CreatureAnimationController : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField, Required]
        private ICreature owner;
        [SerializeField, Required]
        private Animator animator;
        [SerializeField, Required]
        private NavMeshAgent agent;
        [SerializeField, Required]
        private RigBuilder rigBuilder;

        private const string RIGHT_ARM = "CC_Base_R_Upperarm", RIGHT_FOREARM = "CC_Base_R_Forearm", RIGHT_HAND = "CC_Base_R_Hand", RIGHT_HINT = "CC_Base_R_Forearm_Hint";
        private const string LEFT_ARM = "CC_Base_L_Upperarm", LEFT_FOREARM = "CC_Base_L_Forearm", LEFT_HAND = "CC_Base_L_Hand", LEFT_HINT = "CC_Base_L_Forearm_Hint";

        private Transform rightHint, leftHint;
        private TwoBoneIKConstraint rightHandConstraint, leftHandConstraint;

        private bool _initializedOnce = false;

        private void Start()
        {
            rigBuilder.layers.Clear();
            rigBuilder.enabled = false;
        }

        private void OnDestroy()
        {

        }

        private Rig InitializeRigLayer()
        {
            Rig layer = new GameObject("RigLayer").AddComponent<Rig>();
            layer.transform.SetParent(rigBuilder.transform);
            UMAData umaData = owner.References.GetComponentInChildren<UMAData>();

            rightHandConstraint = new GameObject("RightHandIK").AddComponent<TwoBoneIKConstraint>();
            rightHandConstraint.transform.parent = layer.transform;
            rightHandConstraint.data.root = umaData.GetBoneGameObject(RIGHT_ARM).transform;
            Transform rightForearm = umaData.GetBoneGameObject(RIGHT_FOREARM).transform;
            rightHandConstraint.data.mid = rightForearm;
            rightHandConstraint.data.tip = umaData.GetBoneGameObject(RIGHT_HAND).transform;
            rightHandConstraint.data.hint = rightHint = new GameObject(RIGHT_HINT).transform;
            rightHint.parent = rightForearm;

            leftHandConstraint = new GameObject("LeftHandIK").AddComponent<TwoBoneIKConstraint>();
            leftHandConstraint.transform.parent = layer.transform;
            leftHandConstraint.data.root = umaData.GetBoneGameObject(LEFT_ARM).transform;
            Transform leftForearm = umaData.GetBoneGameObject(LEFT_FOREARM).transform;
            leftHandConstraint.data.mid = leftForearm;
            leftHandConstraint.data.tip = umaData.GetBoneGameObject(LEFT_HAND).transform;
            leftHandConstraint.data.hint = leftHint = new GameObject(LEFT_HINT).transform;
            leftHint.parent = leftForearm;

            _initializedOnce = true;
            return layer;
        }

        public void SetCombatAnimationsActive(bool isCombat)
        {
            animator.SetBool("IsCombat", isCombat);
        }

        private void Update()
        {
            animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
        }

        public void EnableIK(WeaponAnimationDataProvider ikData)
        {
            if (!_initializedOnce)
                rigBuilder.layers.Add(new(InitializeRigLayer()));

            rightHandConstraint.data.target = ikData.RightHandIK;
            rightHandConstraint.data.targetPositionWeight = 1;
            rightHandConstraint.data.targetRotationWeight = 1;
            rightHint.localPosition = ikData.RightHintLocalPosition;
            rightHandConstraint.data.hintWeight = 1;

            leftHandConstraint.data.target = ikData.LeftHandIK;
            leftHandConstraint.data.targetPositionWeight = 1;
            leftHandConstraint.data.targetRotationWeight = 1;
            leftHint.localPosition = ikData.RightHintLocalPosition;
            leftHandConstraint.data.hintWeight = 1;

            rigBuilder.enabled = true;
            animator.SetInteger("WeaponAnimationType", (int)ikData.AnimationType);
        }

        public void DisableIK()
        {
            if (!_initializedOnce)
                rigBuilder.layers.Add(new(InitializeRigLayer()));

            rightHandConstraint.data.targetPositionWeight = 0;
            rightHandConstraint.data.targetRotationWeight = 0;
            rightHandConstraint.data.hintWeight = 0;

            leftHandConstraint.data.targetPositionWeight = 0;
            leftHandConstraint.data.targetRotationWeight = 0;
            leftHandConstraint.data.hintWeight = 0;

            rigBuilder.enabled = false;
        }

        public void SetTrigger(int hash)
        {
            animator.SetTrigger(hash);
        }

        public void SetInteger(int hash, int value)
        {
            animator.SetInteger(hash, value);
        }

        public void SetBool(int hash, bool value)
        {
            animator.SetBool(hash, value);
        }
    }
}
