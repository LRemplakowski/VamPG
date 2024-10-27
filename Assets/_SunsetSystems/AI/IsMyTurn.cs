using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Turn System")]
public class IsMyTurn : Conditional
{
    [SerializeField]
	private SharedAIContext _aiContext;

    public override TaskStatus OnUpdate()
	{
        if (_aiContext.Value.IsMyTurn())
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
	}
}