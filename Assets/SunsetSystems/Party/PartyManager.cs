using Entities.Characters;
using Entities.Characters.Data;
using SunsetSystems.Data;
using System.Threading.Tasks;
using UI.CharacterPortraits;
using UnityEngine;
using SunsetSystems.Utils;
using Utils.Threading;

namespace SunsetSystems.Party
{
    public class PartyManager : InitializedSingleton<PartyManager>
    {
        [SerializeField, ReadOnly]
        private Creature[] _currentPartyMembers;
        private Creature[] CurrentPartyMembers { get => _currentPartyMembers; set => _currentPartyMembers = value; }
        [SerializeField]
        private PartyPortraitsController partyPortraits;

        public override void Initialize()
        {
            Dispatcher.Instance.Invoke(async () =>
            {
                CreatePartyList();
                if (partyPortraits == null)
                    partyPortraits = FindObjectOfType<PartyPortraitsController>();
                partyPortraits.Clear();
                foreach (Creature c in CurrentPartyMembers)
                {
                    partyPortraits.AddPortrait(c.GetCreatureUIData());
                    await Task.Yield();
                }
            });
        }

        private void CreatePartyList()
        {
            CurrentPartyMembers = GameRuntimeData.GetActivePartyCreatures().ToArray();
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
