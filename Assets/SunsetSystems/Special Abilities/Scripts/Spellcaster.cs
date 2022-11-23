using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;
using static SunsetSystems.Spellbook.DisciplinePower;

namespace SunsetSystems.Spellbook
{
    public static class Spellcaster
    {
        public static Action GetPowerAction(DisciplinePower power, Creature castingActor)
        {
            return power.Target switch
            {
                Spellbook.Target.Self => () => castingActor.SpellbookManager.UsePower(power, castingActor),
                Spellbook.Target.Friendly => () => castingActor.SpellbookManager.UsePowerAfterTargetSelection(power),
                Spellbook.Target.Hostile => () => castingActor.SpellbookManager.UsePowerAfterTargetSelection(power),
                Spellbook.Target.AOE_Friendly => throw new NotImplementedException(),
                Spellbook.Target.AOE_Hostile => throw new NotImplementedException(),
                _ => null,
            };
        }

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
            List<EffectWrapper> effects = discipline.GetEffects();
            foreach (EffectWrapper effect in effects)
            {
                switch (effect.EffectType)
                {
                    case EffectType.Attribute:
                        HandleAttributeEffect(effect.attributeEffect, target, caster);
                        break;
                    case EffectType.Skill:
                        HandleSkillEffect(effect.skillEffect, target, caster);
                        break;
                    case EffectType.Discipline:
                        HandleDisciplineEffect(effect.disciplineEffect, target, caster);
                        break;
                    case EffectType.Tracker:
                        HandleTrackerEffect(effect.trackerEffect, target, caster);
                        break;
                    case EffectType.ScriptDriven:
                        HandleScriptEffect(effect.scriptEffect, target, caster);
                        break;
                }
            }
        }

        private static void HandleSelfTargeted(DisciplinePower discipline, Creature caster)
        {
            Debug.Log("Using self-targeted discipline " + discipline.name + "! Caster is " + caster.gameObject.name);
            List<EffectWrapper> effects = discipline.GetEffects();
            foreach (EffectWrapper effect in effects)
            {
                switch (effect.EffectType)
                {
                    case EffectType.Attribute:
                        HandleAttributeEffect(effect.attributeEffect, caster, caster);
                        break;
                    case EffectType.Skill:
                        HandleSkillEffect(effect.skillEffect, caster, caster);
                        break;
                    case EffectType.Discipline:
                        HandleDisciplineEffect(effect.disciplineEffect, caster, caster);
                        break;
                    case EffectType.Tracker:
                        HandleTrackerEffect(effect.trackerEffect, caster, caster);
                        break;
                    case EffectType.ScriptDriven:
                        HandleScriptEffect(effect.scriptEffect, caster, caster);
                        break;
                }
            }
        }

        static void HandleAttributeEffect(EffectWrapper.AttributeEffect attributeEffect, Creature target, Creature caster)
        {
            target.StatsManager.ApplyEffect(attributeEffect);
        }

        static void HandleScriptEffect(EffectWrapper.ScriptEffect scriptEffect, Creature target, Creature caster)
        {
            scriptEffect.Script.Activate(target, caster);
        }

        static void HandleTrackerEffect(EffectWrapper.TrackerEffect trackerEffect, Creature target, Creature caster)
        {
            throw new NotImplementedException();
        }

        static void HandleDisciplineEffect(EffectWrapper.DisciplineEffect disciplineEffect, Creature target, Creature caster)
        {
            throw new NotImplementedException();
        }

        static void HandleSkillEffect(EffectWrapper.SkillEffect skillEffect, Creature target, Creature caster)
        {
            throw new NotImplementedException();
        }
    }
}