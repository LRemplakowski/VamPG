using SunsetSystems.Combat;
using System;
using UnityEngine;

[Serializable]
public abstract class DisciplineScript : ScriptableObject, IDisciplineScript
{
    public abstract void Activate(ICombatant target, ICombatant caster);
}

public interface IDisciplineScript
{
    void Activate(ICombatant target, ICombatant caster);
}