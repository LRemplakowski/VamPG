using Entities.Characters;
using UnityEngine;

public abstract class DisciplineScript : ScriptableObject
{
    public abstract void Activate(Creature target, Creature caster);
}