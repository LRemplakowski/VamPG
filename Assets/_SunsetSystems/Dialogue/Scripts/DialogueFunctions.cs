using SunsetSystems.Dice;
using SunsetSystems.Journal;
using SunsetSystems.Party;
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
            return attributeTypeString switch
            {
                "strength" => AttributeType.Strength,
                "dexterity" => AttributeType.Dexterity,
                "stamina" => AttributeType.Stamina,
                "charisma" => AttributeType.Charisma,
                "manipulation" => AttributeType.Manipulation,
                "composure" => AttributeType.Composure,
                "intelligence" => AttributeType.Intelligence,
                "wits" => AttributeType.Wits,
                "resolve" => AttributeType.Resolve,
                _ => AttributeType.Invalid,
            };
        }

        private static SkillType GetSkillTypeFromString(string skillTypeString)
        {
            return skillTypeString switch
            {
                "melee" => SkillType.Melee,
                "firearms" => SkillType.Firearms,
                _ => SkillType.Invalid,
            };
        }

        [YarnFunction("neg")]
        public static int NegativeValue(int value)
        {
            return -value;
        }
    }
}
