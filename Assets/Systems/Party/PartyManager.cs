using Entities.Characters;
using Entities.Characters.Data;
using SunsetSystems.Management;
using System;
using UI.CharacterPortraits;
using UnityEngine;

namespace SunsetSystems.Party
{
    public class PartyManager : Manager
    {
        [SerializeField, ReadOnly]
        private Creature[] _currentPartyMembers;
        private Creature[] CurrentPartyMembers { get => _currentPartyMembers; set => _currentPartyMembers = value; }
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
            CurrentPartyMembers = FindObjectsOfType<PlayerControlledCharacter>();
            Array.Sort(CurrentPartyMembers);
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
    }
}
