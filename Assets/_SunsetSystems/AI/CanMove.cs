using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;


[TaskCategory("Turn System/")]
public class CanMove : Conditional
{
	[SerializeField, SharedRequired]
	private SharedAIContext _aiContext;

	public override TaskStatus OnUpdate()
	{
        return HasEnoughMovementPoints() ? TaskStatus.Success : TaskStatus.Failure;
    }

    private bool HasEnoughMovementPoints()
    {
        return _aiContext.Value.CanMove();
    }
}