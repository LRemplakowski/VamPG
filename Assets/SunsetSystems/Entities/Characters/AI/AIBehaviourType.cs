using UnityEngine;

[CreateAssetMenu(fileName = "New Behaviour", menuName = "Character/AI Behaviour")]
public class AIBehaviourType : ScriptableObject
{
    [SerializeField]
    private BehaviourType _baseType = BehaviourType.Default;
    public BehaviourType BaseType { get => _baseType; }

    public enum BehaviourType
    {
        Aggressive, Defensive, Support, Default
    }
}