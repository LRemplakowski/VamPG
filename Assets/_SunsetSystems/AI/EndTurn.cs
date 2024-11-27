using BehaviorDesigner.Runtime.Tasks;
using SunsetSystems.Combat;
using UnityEngine;

[TaskCategory("Turn System")]
public class EndTurn : Action
{
    [SerializeField, SharedRequired]
    private SharedAIContext _aiContext;

    public override void OnStart()
	{
		_aiContext.Value.GetCombatant().SignalEndTurn();
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}