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
        if (_aiContext.Value.CanMove())
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}