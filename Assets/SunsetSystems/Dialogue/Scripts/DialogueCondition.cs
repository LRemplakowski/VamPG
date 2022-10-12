using Entities.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Dice;
using SunsetSystems.Game;
using SunsetSystems.Party;

namespace SunsetSystems.Dialogue
{
    public class DialogueCondition
    {
        private static readonly DialogueCondition instance = new DialogueCondition();

        private DialogueCondition()
        {

        }

        public static bool Evaluate(string condition)
        {
            MethodInvoker.ParseMethod(condition, out string methodName, out List<object> args);
            MethodInvoker.InvokeConditional(instance, methodName, args, out bool result);
            Debug.Log(methodName + ", " + args + ", " + result);
            return result;
        }

        public static bool PlayerSex(string sex)
        {
            if (Enum.TryParse(sex, out BodyType result))
            {
                return PartyManager.MainCharacter.Data.bodyType.Equals(result);
            }
            else
                return false;
        }

        public static bool PlayerType(string type)
        {
            if (Enum.TryParse(type, out CreatureType result))
            {
                return PartyManager.MainCharacter.Data.creatureType.Equals(result);
            }
            else
                return false;
        }

        public static bool PlayerHasDisciplinePower(string powerName)
        {
            DisciplinePower p = PartyManager.MainCharacter.StatsManager.GetDisciplinePower(powerName);
            return !p.Type.Equals(DisciplineType.Invalid);
        }

        //TODO implement
        public static bool PartyHasDisciplinePower(string powerName)
        {
            return true;
        }

        public static bool PlayerPassedSkillCheck(string attributeType, string skillType, int dc)
        {
            StatsManager playerStatsManager = PartyManager.MainCharacter.StatsManager;
            if (Enum.TryParse(attributeType, out AttributeType attribute) && Enum.TryParse(skillType, out SkillType skill))
            {
                Outcome outcome = playerStatsManager.GetSkillRoll(attribute, skill);
                return outcome.successes >= dc;
            }
            return false;
        }
    }
}
