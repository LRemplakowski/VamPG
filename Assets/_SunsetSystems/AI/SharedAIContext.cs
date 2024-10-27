using BehaviorDesigner.Runtime;
using SunsetSystems.AI;

[System.Serializable]
public class SharedAIContext : SharedVariable<AIBehaviourContext>
{
	public override string ToString() { return mValue == null ? "null" : mValue.ToString(); }
	public static implicit operator SharedAIContext(AIBehaviourContext value) { return new SharedAIContext { mValue = value }; }
}