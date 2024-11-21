using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Grid Combat")]
public class ShouldKeepCurrentPosition : Conditional
{
    [SerializeField]
    private float _keepPositionThreshold = 100f;
    [SerializeField, Range(1, 100)]
    private float _inCoverScore = 10, _currentPositionScore = 10, _hasTargetsInRange = 10f;

    [SerializeField]
    private SharedAIContext _aiContext;

    public override TaskStatus OnUpdate()
	{
        float targetsScore = _aiContext.Value.GetTargetsInWeaponRange() * _hasTargetsInRange;
        float coverScore = _aiContext.Value.IsInCover() ? _inCoverScore : 0;
        float totalScore = _currentPositionScore + coverScore + targetsScore;
        if (EvaluateScore(_keepPositionThreshold, totalScore))
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
	}

    private bool EvaluateScore(float score, float threshold)
    {
        return score >= threshold;
    }
}