using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using static SunsetSystems.Spellbook.DisciplinePower;

namespace SunsetSystems.Spellbook
{
    public static class Spellcaster
    {
        public static bool HandleEffects(DisciplinePower discipline, IMagicUser caster)
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

        public static bool HandleEffects(DisciplinePower discipline, IMagicUser caster, ICombatant target)
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

        public static bool HandleEffects(DisciplinePower discipline, IMagicUser caster, Vector3 originPoint)
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

        private static void HandleAOE(DisciplinePower discipline, IMagicUser caster, Vector3 originPoint)
        {
            Debug.Log("Using AOE discipline " + discipline.name + "! Caster is " + caster.References.GameObject.name + " and origin point is " + originPoint);
        }

        private static void HandleSingleTargeted(DisciplinePower discipline, IMagicUser caster, ICombatant target)
        {
            Debug.Log("Using single targeted discipline " + discipline.name + "! Caster is " + caster.References.GameObject.name + " and target is " + target.References.GameObject.name);
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

        private static void HandleSelfTargeted(DisciplinePower discipline, IMagicUser caster)
        {
            Debug.Log("Using self-targeted discipline " + discipline.name + "! Caster is " + caster.References.GameObject.name);
            List<EffectWrapper> effects = discipline.GetEffects();
            foreach (EffectWrapper effect in effects)
            {
                switch (effect.EffectType)
                {
                    case EffectType.Attribute:
                        HandleAttributeEffect(effect.attributeEffect, caster.References.CombatBehaviour, caster);
                        break;
                    case EffectType.Skill:
                        HandleSkillEffect(effect.skillEffect, caster.References.CombatBehaviour, caster);
                        break;
                    case EffectType.Discipline:
                        HandleDisciplineEffect(effect.disciplineEffect, caster.References.CombatBehaviour, caster);
                        break;
                    case EffectType.Tracker:
                        HandleTrackerEffect(effect.trackerEffect, caster.References.CombatBehaviour, caster);
                        break;
                    case EffectType.ScriptDriven:
                        HandleScriptEffect(effect.scriptEffect, caster.References.CombatBehaviour, caster);
                        break;
                }
            }
        }

        static void HandleAttributeEffect(EffectWrapper.AttributeEffect attributeEffect, ICombatant target, IMagicUser caster)
        {
            throw new NotImplementedException();
        }

        static void HandleScriptEffect(EffectWrapper.ScriptEffect scriptEffect, ICombatant target, IMagicUser caster)
        {
            throw new NotImplementedException();
        }

        static void HandleTrackerEffect(EffectWrapper.TrackerEffect trackerEffect, ICombatant target, IMagicUser caster)
        {
            throw new NotImplementedException();
        }

        static void HandleDisciplineEffect(EffectWrapper.DisciplineEffect disciplineEffect, ICombatant target, IMagicUser caster)
        {
            throw new NotImplementedException();
        }

        static void HandleSkillEffect(EffectWrapper.SkillEffect skillEffect, ICombatant target, IMagicUser caster)
        {
            throw new NotImplementedException();
        }
    }
}