using Entities.Characters;
using Entities.Characters.Data;
using SunsetSystems.Data;
using SunsetSystems.Management;
using System.Threading.Tasks;
using UI.CharacterPortraits;
using UnityEngine;
using Utils;
using Utils.Threading;

namespace SunsetSystems.Party
{
    public class PartyManager : Manager, IInitialized
    {
        [SerializeField, ReadOnly]
        private Creature[] _currentPartyMembers;
        private Creature[] CurrentPartyMembers { get => _currentPartyMembers; set => _currentPartyMembers = value; }
        [SerializeField]
        private PartyPortraitsController partyPortraits;

        public Task Initialize()
        {
            return Task.Run(async () =>
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
                await Task.Yield();
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
