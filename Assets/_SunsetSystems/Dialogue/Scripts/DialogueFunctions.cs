using SunsetSystems.Dice;
using SunsetSystems.Inventory;
using SunsetSystems.Journal;
using SunsetSystems.Party;
using System;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class DialogueFunctions
    {
        [YarnFunction("Roll")]
        public static int GetRollResult(string statName)
        {
            int dice = 0;
            dice += GetStatValueFromString(statName);
            Outcome rollOutcome = Roll.d10(dice);
            return rollOutcome.successes;
        }

        [YarnFunction("CurrentMoney")]
        public static float GetCurrentMoney()
        {
            return InventoryManager.Instance.GetMoneyAmount();
        }

        private static int GetStatValueFromString(string statName)
        {
            AttributeType attributeType = GetAttributeTypeFromString(statName);
            if (attributeType != AttributeType.Invalid)
            {
                return PartyManager.MainCharacter.Data.Stats.Attributes.GetAttribute(attributeType).GetValue();
            }
            SkillType skillType = GetSkillTypeFromString(statName);
            if (skillType != SkillType.Invalid)
            {
                return PartyManager.MainCharacter.Data.Stats.Skills.GetSkill(skillType).GetValue();
            }
            return 1;
        }

        private static AttributeType GetAttributeTypeFromString(string attributeTypeString)
        {
            if (Enum.TryParse(attributeTypeString, true, out AttributeType result))
            {
                return result;
            }
            return AttributeType.Invalid;
        }

        private static SkillType GetSkillTypeFromString(string skillTypeString)
        {
            if (Enum.TryParse(skillTypeString, true, out SkillType result))
            {
                return result;
            }
            return SkillType.Invalid;
        }

        [YarnFunction("neg")]
        public static int NegativeValue(int value)
        {
            return -value;
        }
    }
}
