using SunsetSystems.Entities.Characters;
using System;
using UnityEngine;

[Serializable]
public abstract class DisciplineScript : ScriptableObject, IDisciplineScript
{
    public abstract void Activate(Creature target, Creature caster);
}

public interface IDisciplineScript
{
    void Activate(Creature target, Creature caster);
}