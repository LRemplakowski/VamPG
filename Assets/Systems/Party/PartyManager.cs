using Entities.Characters;
using Entities.Characters.Data;
using SunsetSystems.Journal;
using SunsetSystems.Management;
using UI.CharacterPortraits;
using UnityEngine;

namespace SunsetSystems.Party
{
    public class PartyManager : Manager
    {
        [SerializeField, ReadOnly]
        private Creature[] _currentPartyMembers;
        private Creature[] CurrentPartyMembers { get => _currentPartyMembers; set => _currentPartyMembers = value; }
        [SerializeField, ReadOnly]
        private Creature[] _reservePartyMembers;
        private Creature[] ReservePartyMembers { get => _reservePartyMembers; set => _reservePartyMembers = value; }
        [SerializeField]
        private PartyPortraitsController partyPortraits;

        private void Initialize()
        {
            CreatePartyList();
            if (partyPortraits == null)
                partyPortraits = FindObjectOfType<PartyPortraitsController>();
            foreach (Creature c in CurrentPartyMembers)
            {
                partyPortraits.AddPortrait(c.GetCreatureUIData());
            }
        }

        private void OnEnable()
        {
            MainCharacter.onMainCharacterInitialized += Initialize;
        }

        private void OnDisable()
        {
            MainCharacter.onMainCharacterInitialized -= Initialize;
        }

        private void CreatePartyList()
        {
            GameJournal journal = FindObjectOfType<GameJournal>();
            CurrentPartyMembers = new Creature[journal.ActiveCompanions.Length + 1];
            CurrentPartyMembers[0] = journal.PlayerCharacterData.GetComponent<Creature>();
            for (int i = 1; i < journal.ActiveCompanions.Length + 1; i++)
            {
                CurrentPartyMembers[i] = journal.ActiveCompanions[i].GetComponent<Creature>();
            }
        }

        public CreatureUIData[] GetCurrentMembersData()
        {
            CreatureUIData[] currentMembersData = new CreatureUIData[CurrentPartyMembers.Length];
            for (int i = 0; i < currentMembersData.Length; i++)
            {
                currentMembersData[i] = CurrentPartyMembers[i].GetCreatureUIData();
            }
            return currentMembersData;
        }

        public CreatureUIData[] GetReserveMembersData()
        {
            CreatureUIData[] reserveMembersData = new CreatureUIData[ReservePartyMembers.Length];
            for (int i = 0; i < reserveMembersData.Length; i++)
            {
                reserveMembersData[i] = ReservePartyMembers[i].GetCreatureUIData();
            }
            return reserveMembersData;
        }
    }
}
