using Entities.Characters;
using Entities.Characters.Data;
using Systems.Management;
using UI.CharacterPortraits;
using UnityEngine;

namespace Systems.Party
{
    public class PartyManager : Manager
    {
        private Creature[] CurrentPartyMembers { get; set; }
        private Creature[] ReservePartyMembers { get; set; }
        [SerializeField]
        private PartyPortraitsController partyPortraits;

        private void Start()
        {
            CreatePartyList();
            if (partyPortraits == null)
                partyPortraits = FindObjectOfType<PartyPortraitsController>();
            foreach (Creature c in CurrentPartyMembers)
            {
                partyPortraits.AddPortrait(c.GetCreatureUIData());
            }
        }

        private void CreatePartyList()
        {
            CurrentPartyMembers = FindObjectsOfType<Player>();
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
