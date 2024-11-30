using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Turn System")]
public class ExecuteSelectedAction : Action
{
    [SerializeField, SharedRequired]
    private SharedAIContext _aiContext;

	private bool _actionFinished = false;
	private bool _executionFailed = false;

    public override void OnStart()
	{
		_actionFinished = false;
		var context = _aiContext.Value;
		_executionFailed = !context.GetAbilityUser().ExecutAbility(context.SelectedAbility, context.SelectedTarget, OnExecutionFinished);
	}

	public override TaskStatus OnUpdate()
	{
		if (_executionFailed)
		{
			return TaskStatus.Failure;
		}
		if (_actionFinished)
        {
            return TaskStatus.Success;
        }
		else
		{
			return TaskStatus.Running;
		}
    }

	private void OnExecutionFinished()
	{
		_actionFinished = true;
	}
}