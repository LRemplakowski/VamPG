using Entities.Characters;
using SunsetSystems.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SunsetSystems.Management;
using Utils;
using SunsetSystems.Input.CameraControl;

namespace SunsetSystems.Loading
{
    internal abstract class DefaultSceneLogic : AbstractSceneLogic
    {
        [SerializeField, ES3NonSerializable]
        private GameRuntimeData _gameRuntimeData;

        public async override Task StartSceneAsync(string entryPointTag)
        {
            Debug.Log("Starting scene");
            if (!_gameRuntimeData)
                _gameRuntimeData = FindObjectOfType<GameRuntimeData>();
            CreatureAsset mainCharAsset = _gameRuntimeData.MainCharacterAsset;
            List<CreatureAsset> activeParty = _gameRuntimeData.ActivePartyAssets;
            List<Vector3> partyPositions = _gameRuntimeData.ActivePartySavedPositions;
            if (partyPositions != null && partyPositions.Count > 0)
            {
                await InstantiateParty(partyPositions, mainCharAsset, activeParty);
            }
            else
            {
                AreaEntryPoint entryPoint = FindAreaEntryPoint(entryPointTag);
                await InstantiateParty(entryPoint.transform.position, mainCharAsset, activeParty);
            }
            foreach (IInitialized initable in References.GetAll<IInitialized>())
            {
                await initable.Initialize();
            }
        }

        public async override Task StartSceneAsync()
        {
            await StartSceneAsync("");
        }

        protected async Task InstantiateParty(Vector3 position, CreatureAsset mainChar, List<CreatureAsset> party)
        {
            List<Vector3> positions = PlayerInputHandler.GetPositionsFromPoint(position);
            await InstantiateParty(positions, mainChar, party);
        }

        protected async Task InstantiateParty(List<Vector3> positions, CreatureAsset mainChar, List<CreatureAsset> party)
        {
            Debug.Log("Initializing party");
            CreatureData mainCharData = null;
            List<CreatureData> partyData = new();
            if (mainChar != null)
            {
                mainCharData = InitializePartyMember(mainChar, positions[0]);
                for (int i = 0; i < party.Count; i++)
                {
                    partyData.Add(InitializePartyMember(party[i], positions[i + 1]));
                    await Task.Yield();
                }
            }
            _gameRuntimeData.MainCharacterData = mainCharData;
            _gameRuntimeData.ActivePartyData.Clear();
            _gameRuntimeData.ActivePartyData.AddRange(partyData);
            FindObjectOfType<CameraControlScript>().ForceToPosition(positions[0]);
        }

        protected CreatureData InitializePartyMember(CreatureAsset asset, Vector3 position)
        {
            CreatureData creatureData = Instantiate(creaturePrefab, position, Quaternion.identity);
            creatureData.SetData(asset);
            return creatureData;
        }

        private AreaEntryPoint FindAreaEntryPoint(string tag)
        {
            AreaEntryPoint entryPoint = null;
            if (!tag.Equals(""))
            {
                if (this.TryFindFirstWithTag(tag, out GameObject result))
                    entryPoint = result.GetComponent<AreaEntryPoint>();
            }
            if (entryPoint == null)
            {
                entryPoint = FindObjectOfType<AreaEntryPoint>();
            }
            return entryPoint;
        }

        public sealed override void LoadRuntimeData()
        {
            ES3.LoadInto(unique.Id, this);
        }

        public sealed override void SaveRuntimeData()
        {
            ES3.Save(unique.Id, this);
        }
    }
}
