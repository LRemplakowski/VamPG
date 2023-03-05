using SunsetSystems.Entities.Characters;
using SunsetSystems.Party;
using SunsetSystems.UI.Utils;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

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

        private IPartyManager _partyManager;
        private ICharacterSelector _characterSelector;

        [Inject]
        public void InjectDependencies(ICharacterSelector characterSelector, IPartyManager partyManager)
        {
            _characterSelector = characterSelector;
            _partyManager = partyManager;
        }

        private void OnEnable()
        {
            UpdateCharacterSheet();
        }

        public void UpdateCharacterSheet()
        {
            string selectedCharacterKey = _characterSelector.SelectedCharacterKey;
            CreatureData data = _partyManager.GetPartyMemberDataByID(selectedCharacterKey);
            List<IGameDataProvider<BaseStat>> attributes = new();
            attributes.AddRange(data.Stats.Attributes.GetAttributeList());
            _attributes.ForEach(attributeGroup => attributeGroup.UpdateViews(attributes));
            List<IGameDataProvider<BaseStat>> skills = new();
            skills.AddRange(data.Stats.Skills.GetSkillList());
            _skills.ForEach(skillGroup => skillGroup.UpdateViews(skills));
            List<IGameDataProvider<BaseStat>> disciplines = new();
            disciplines.AddRange(data.Stats.Disciplines.GetDisciplines());
            _disciplines.UpdateViews(disciplines);
        }
    }
}
