using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Turn System")]
public class IsMyTurn : Conditional
{
    [SerializeField, SharedRequired]
	private SharedAIContext _aiContext;

    public override TaskStatus OnUpdate()
	{
        return _aiContext.Value.IsMyTurn() ? TaskStatus.Success : TaskStatus.Failure;
    }
}