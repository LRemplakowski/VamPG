using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

[TaskCategory("Turn System")]
public class CanAct : Conditional
{
    [SerializeField, SharedRequired]
    private SharedAIContext _aiContext;

	public override TaskStatus OnUpdate()
	{
        return HasEnoughActionPoints() ? TaskStatus.Success : TaskStatus.Failure;
    }

	private bool HasEnoughActionPoints()
	{
		return _aiContext.Value.GetHasEnoughActionPoints(_aiContext.Value.SelectedAbility);
	}
}