using SunsetSystems.Entities.Characters;
using NaughtyAttributes;
using SunsetSystems.Combat;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using SunsetSystems.Animation;
using UMA;

public class CreatureAnimationController : MonoBehaviour
{
    const float movementAnimationSmoothTime = 0.1f;

    [SerializeField, ReadOnly]
    private Animator animator;
    [SerializeField, ReadOnly]
    private NavMeshAgent agent;
    [SerializeField, ReadOnly]
    private RigBuilder rigBuilder;

    private const string RIGHT_ARM = "CC_Base_R_Upperarm", RIGHT_FOREARM = "CC_Base_R_Forearm", RIGHT_HAND = "CC_Base_R_Hand", RIGHT_HINT = "CC_Base_R_Forearm_Hint";
    private const string LEFT_ARM = "CC_Base_L_Upperarm", LEFT_FOREARM = "CC_Base_L_Forearm", LEFT_HAND = "CC_Base_L_Hand", LEFT_HINT = "CC_Base_L_Forearm_Hint";

    private Transform rightHint, leftHint;
    private TwoBoneIKConstraint rightHandConstraint, leftHandConstraint;

    private bool _initializedOnce = false;

    private void OnEnable()
    {
        CombatManager.CombatBegin += OnCombatStart;
        CombatManager.CombatEnd += OnCombatEnd;
    }

    private void OnDisable()
    {
        CombatManager.CombatBegin -= OnCombatStart;
        CombatManager.CombatEnd -= OnCombatEnd;
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rigBuilder = GetComponent<RigBuilder>();
        rigBuilder.enabled = false;
    }

    private Rig InitializeRigLayer()
    {
        Rig layer = Instantiate(new GameObject("RigLayer"), transform).AddComponent<Rig>();
        UMAData umaData = GetComponent<UMAData>();

        rightHandConstraint = Instantiate(new GameObject("RightHandIK"), layer.transform).AddComponent<TwoBoneIKConstraint>();
        rightHandConstraint.data.root = umaData.GetBoneGameObject(RIGHT_ARM).transform;
        Transform rightForearm = umaData.GetBoneGameObject(RIGHT_FOREARM).transform;
        rightHandConstraint.data.mid = rightForearm;
        rightHandConstraint.data.tip = umaData.GetBoneGameObject(RIGHT_HAND).transform;
        rightHandConstraint.data.hint = rightHint = Instantiate(new GameObject(RIGHT_HINT), rightForearm).transform;

        leftHandConstraint = Instantiate(new GameObject("LeftHandIK"), layer.transform).AddComponent<TwoBoneIKConstraint>();
        leftHandConstraint.data.root = umaData.GetBoneGameObject(LEFT_ARM).transform;
        Transform leftForearm = umaData.GetBoneGameObject(LEFT_FOREARM).transform;
        leftHandConstraint.data.mid = leftForearm;
        leftHandConstraint.data.tip = umaData.GetBoneGameObject(LEFT_HAND).transform;
        leftHandConstraint.data.hint = leftHint = Instantiate(new GameObject(LEFT_HINT), leftForearm).transform;

        _initializedOnce = true;
        return layer;
    }

    private void OnCombatStart(List<Creature> creaturesInCombat)
    {
        animator.SetBool("IsCombat", true);
    }

    private void OnCombatEnd()
    {
        animator.SetBool("IsCombat", false);
    }

    private void Update()
    {
        float speedPercentage = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("Speed", speedPercentage);
    }

    public void EnableIK(AnimationDataProvider ikData)
    {
        if (!_initializedOnce)
            rigBuilder.layers.Add(new(InitializeRigLayer()));
        rightHandConstraint.data.target = ikData.RightHandIK;
        rightHint.localPosition = ikData.RightHintLocalPosition;
        leftHandConstraint.data.target = ikData.LeftHandIK;
        leftHint.localPosition = ikData.LeftHintLocalPosition;
        rigBuilder.enabled = true;
    }

    public void DisableIK()
    {
        rigBuilder.enabled = false;
    }
}
