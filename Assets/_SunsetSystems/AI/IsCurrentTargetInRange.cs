using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Turn System")]
public class IsCurrentTargetInRange : Conditional
{
    [SerializeField, SharedRequired]
    private SharedAIContext _aiContext;

    public override TaskStatus OnUpdate()
	{
        return IsTargetInRange() ? TaskStatus.Success : TaskStatus.Failure;
	}

    private bool IsTargetInRange()
    {
        return _aiContext.Value.IsCurrentTargetInAbilityRange();
    }
}