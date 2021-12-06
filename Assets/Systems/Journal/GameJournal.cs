using Entities.Characters;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

namespace SunsetSystems.Journal
{
    public class GameJournal : Singleton<GameJournal>
    {
        [SerializeField]
        private CreatureData creaturePrefab;
        [SerializeField]
        private CreatureData _playerCharacterData;
        public CreatureData PlayerCharacterData 
        { 
            get => _playerCharacterData; 
        }
        [SerializeField]
        private CreatureData[] _activeCompanions = new CreatureData[0];
        public CreatureData[] ActiveCompanions { get => _activeCompanions; }

        private PartyAssetsWrapper _partyAssets;
        /// <summary>
        /// Can only be set once, if null.
        /// </summary>
        internal PartyAssetsWrapper PartyAssets
        {
            private get => _partyAssets;
            set
            {
                if (_partyAssets == null)
                {
                    _partyAssets = value;
                }
            }
        }

        public void InitializeParty(Vector3 position)
        {
            if (PartyAssets != null)
            {
                _playerCharacterData = Instantiate(creaturePrefab, position, Quaternion.identity);
                _playerCharacterData.SetData(PartyAssets.mainCharacterAsset);
                _playerCharacterData.CreateCreature();
            }
        }
    }

    internal class PartyAssetsWrapper
    {
        internal readonly CreatureAsset mainCharacterAsset;
        internal readonly List<CreatureAsset> recruitedCompanionsAssets = new List<CreatureAsset>();

        public PartyAssetsWrapper(CreatureAsset mainCharacterAsset)
        {
            this.mainCharacterAsset = mainCharacterAsset;
        }
    }
}
