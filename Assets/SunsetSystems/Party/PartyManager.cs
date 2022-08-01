using Entities.Characters;
using Entities.Characters.Data;
using SunsetSystems.Data;
using UI.CharacterPortraits;
using UnityEngine;
using SunsetSystems.Utils;
using System.Collections.Generic;

namespace SunsetSystems.Party
{
    public class PartyManager : InitializedSingleton<PartyManager>
    {
        [SerializeField, ReadOnly]
        private List<Creature> _currentPartyMembers;
        private List<Creature> CurrentPartyMembers { get => _currentPartyMembers; set => _currentPartyMembers = value; }
        private PartyPortraitsController _partyPortraits;
        private PartyPortraitsController PartyPortraits
        {
            get
            {
                if (!_partyPortraits)
                    _partyPortraits = this.FindFirstComponentWithTag<PartyPortraitsController>(TagConstants.PARTY_PORTRAITS_CONTROLLER);
                return _partyPortraits;
            }
        }

        public override void Initialize()
        {
            CreatePartyList();
            PartyPortraits.Clear();
            Debug.Log("Party members count: " + CurrentPartyMembers.Count);
            CurrentPartyMembers.ForEach(c => PartyPortraits.AddPortrait(c.GetCreatureUIData()));
        }

        private void CreatePartyList()
        {
            CurrentPartyMembers = GameRuntimeData.GetActivePartyCreatures();
        }
    }
}
