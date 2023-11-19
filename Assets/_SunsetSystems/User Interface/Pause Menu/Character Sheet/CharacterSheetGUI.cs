using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Data;
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
        [SerializeField]
        private DisciplineGroupUpdateReciever _disciplines;

        private void OnEnable()
        {
            UpdateCharacterSheet();
        }

        public void UpdateCharacterSheet()
        {
            string selectedCharacterKey = CharacterSelector.SelectedCharacterKey;
            ICreature creature = PartyManager.Instance.GetPartyMemberByID(selectedCharacterKey);
            StatsData data = creature.References.StatsManager.Stats;
            List<IGameDataProvider<BaseStat>> attributes = new();
            attributes.AddRange(data.Attributes?.GetAttributeList() ?? new());
            _attributes.ForEach(attributeGroup => attributeGroup.UpdateViews(attributes));
            List<IGameDataProvider<BaseStat>> skills = new();
            skills.AddRange(data.Skills?.GetSkillList() ?? new());
            _skills.ForEach(skillGroup => skillGroup.UpdateViews(skills));
            List<IGameDataProvider<BaseStat>> disciplines = new();
            //disciplines.AddRange(data.Disciplines?.GetDisciplines() ?? new());
            //_disciplines.UpdateViews(disciplines);
        }
    }
}
