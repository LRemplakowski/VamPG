using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.CharacterPortraits;
using UnityEngine;

namespace Systems.Party
{
    public class PartyManager : ExposableMonobehaviour
    {
        private Creature[] PartyMembers { get; set; }
        [SerializeField]
        private PartyPortraitsController partyPortraits;

        private void Start()
        {
            CreatePartyList();
            if (partyPortraits == null)
                partyPortraits = FindObjectOfType<PartyPortraitsController>();
            foreach (Creature c in PartyMembers)
            {
                partyPortraits.AddPortrait(c.GetCreatureUIData());
            }
        }

        private void CreatePartyList()
        {
            PartyMembers = FindObjectsOfType<Player>();
        }
    }
}
