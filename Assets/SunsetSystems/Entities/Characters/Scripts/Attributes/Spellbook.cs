using SunsetSystems.Entities.Characters;
using UnityEngine;

public static class Spellbook
{
    public static bool HandleEffects(DisciplinePower discipline, Creature caster)
    {
        switch (discipline.Target)
        {
            case Target.Self:
                HandleSelfTargeted(discipline, caster);
                break;
            default:
                return false;
        }
        return true;
    }

    public static bool HandleEffects(DisciplinePower discipline, Creature caster, Creature target)
    {
        switch (discipline.Target)
        {
            case Target.Self:
                HandleSelfTargeted(discipline, caster);
                break;
            case Target.Friendly:
                HandleSingleTargeted(discipline, caster, target);
                break;
            case Target.Hostile:
                HandleSingleTargeted(discipline, caster, target);
                break;
            default:
                return false;
        }
        return true;
    }

    public static bool HandleEffects(DisciplinePower discipline, Creature caster, Vector3 originPoint)
    {
        switch (discipline.Target)
        {
            case Target.AOE_Friendly:
                HandleAOE(discipline, caster, originPoint);
                break;
            case Target.AOE_Hostile:
                HandleAOE(discipline, caster, originPoint);
                break;
            default:
                return false;
        }
        return true;
    }

    private static void HandleAOE(DisciplinePower discipline, Creature caster, Vector3 originPoint)
    {
        Debug.Log("Using AOE discipline " + discipline.name + "! Caster is " + caster.gameObject.name + " and origin point is " + originPoint);
    }

    private static void HandleSingleTargeted(DisciplinePower discipline, Creature caster, Creature target)
    {
        Debug.Log("Using single targeted discipline " + discipline.name + "! Caster is " + caster.gameObject.name + " and target is " + target.gameObject.name);
    }

    private static void HandleSelfTargeted(DisciplinePower discipline, Creature caster)
    {
        Debug.Log("Using self-targeted discipline " + discipline.name + "! Caster is " + caster.gameObject.name);
    }
}