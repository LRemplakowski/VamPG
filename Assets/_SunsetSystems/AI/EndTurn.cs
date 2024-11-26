using BehaviorDesigner.Runtime.Tasks;
using SunsetSystems.Combat;

[TaskCategory("Turn System")]
public class EndTurn : Action
{
	public override void OnStart()
	{
		CombatManager.Instance.NextRound();
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}