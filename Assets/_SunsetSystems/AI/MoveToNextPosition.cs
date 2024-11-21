using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SunsetSystems.Combat.Grid;

[TaskCategory("Grid System")]
public class MoveToNextPosition : Action
{
	[SerializeField, SharedRequired]
	private SharedAIContext _aiContext;

	private Awaitable _movementTask;
	private TaskStatus _currentStatus = TaskStatus.Inactive;

	public override void OnStart()
	{
		_currentStatus = TaskStatus.Running;
	}

    public override void OnBehaviorRestart()
    {
        _movementTask?.Cancel();
		_movementTask = null;
    }

    public override TaskStatus OnUpdate()
	{
		if (_movementTask == null)
		{
			_currentStatus = TaskStatus.Failure;
		}
        else if (_movementTask.IsCompleted)
        {
			_currentStatus = TaskStatus.Success;
        }

        return _currentStatus;
	}
}