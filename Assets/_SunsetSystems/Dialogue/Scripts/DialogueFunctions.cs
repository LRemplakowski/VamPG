using SunsetSystems.Dice;
using SunsetSystems.Journal;
using SunsetSystems.Party;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class DialogueFunctions
    {
        [YarnFunction]
        public static string GetPCName()
        {
            return PartyManager.MainCharacter.Data.FullName;
        }

        [YarnFunction("Roll")]
        public static int GetRollResult(string attribute)
        {
            AttributeType attributeType = GetAttributeTypeFromString(attribute);
            int dice = 0;
            dice += PartyManager.MainCharacter.Data.Stats.Attributes.GetAttribute(attributeType).GetValue();
            Outcome rollOutcome = Roll.d10(dice);
            return rollOutcome.successes;
        }

        private static AttributeType GetAttributeTypeFromString(string attributeTypeString)
        {
            return attributeTypeString switch
            {
                "charisma" => AttributeType.Charisma,
                _ => throw new System.NotImplementedException(),
            };
        }

        private static SkillType GetSkillTypeFromString(string skillTypeString)
        {
            return SkillType.Invalid;
        }
    }
}
