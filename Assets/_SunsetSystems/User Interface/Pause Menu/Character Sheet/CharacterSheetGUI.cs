using SunsetSystems.Entities.Characters;
using SunsetSystems.Party;
using SunsetSystems.UI.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class CharacterSheetGUI : MonoBehaviour
    {
        [SerializeField]
        private List<AttributeGroupUpdateReciever> _attributes;
        [SerializeField]
        private List<SkillGroupUpdateReciever> _skills;

        private void OnEnable()
        {
            UpdateCharacterSheet();
        }

        public void UpdateCharacterSheet()
        {
            string selectedCharacterKey = CharacterSelector.SelectedCharacterKey;
            CreatureData data = PartyManager.Instance.GetPartyMemberDataByID(selectedCharacterKey);
            List<IGameDataProvider<BaseStat>> attributes = new();
            attributes.AddRange(data.Stats.Attributes.GetAttributeList());
            _attributes.ForEach(attributeGroup => attributeGroup.UpdateViews(attributes));
            List<IGameDataProvider<BaseStat>> skills = new();
            skills.AddRange(data.Stats.Skills.GetSkillList());
            _skills.ForEach(skillGroup => skillGroup.UpdateViews(skills));
        }
    }
}
