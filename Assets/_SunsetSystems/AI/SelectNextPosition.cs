using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Turn System")]
public class SelectNextPosition : Action
{
    [SerializeField, SharedRequired]
    private SharedAIContext _aiContext;

    public override void OnStart()
	{
		_aiContext.Value.SelectNextPosition();
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}