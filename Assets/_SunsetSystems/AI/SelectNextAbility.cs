using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SunsetSystems.Utils.Extensions;
using System.Linq;
using SunsetSystems.Abilities;

[TaskCategory("Turn System")]
public class SelectNextAbility : Action
{
    [SerializeField, SharedRequired]
    private SharedAIContext _aiContext;

    public override void OnStart()
	{
        var randomAbility = _aiContext.Value.GetAbilityUser()
                                            .GetAllAbilities()
                                            .Where(ability => ability.GetCategories().HasFlag(AbilityCategory.Movement) is false)
                                            .GetRandom();
		_aiContext.Value.SelectedAbility = randomAbility;
	}

	public override TaskStatus OnUpdate()
	{
		return TaskStatus.Success;
	}
}