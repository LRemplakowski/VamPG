using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Turn System")]
public class IsInTurnMode : Conditional
{
    [SerializeField, SharedRequired]
    private SharedAIContext _aiContext;

    public override TaskStatus OnUpdate()
	{
        return _aiContext.Value.IsTurnMode() ? TaskStatus.Success : TaskStatus.Failure;
    }
}