using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SunsetSystems.Utils.Extensions;

[TaskCategory("Turn System")]
public class SelectNextTarget : Action
{
    [SerializeField, SharedRequired]
    private SharedAIContext _aiContext;

    public override void OnStart()
	{
		_aiContext.Value.SelectedTarget = _aiContext.Value.GetAllHostileToMe().GetRandom();
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}