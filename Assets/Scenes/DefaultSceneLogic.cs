using Entities.Characters;
using SunsetSystems.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SunsetSystems.Utils;
using SunsetSystems.Input.CameraControl;
using Glitchers;
using SunsetSystems.Game;

namespace SunsetSystems.Loading
{
    internal class DefaultSceneLogic : AbstractSceneLogic
    {
        [SerializeField, ES3NonSerializable]
        private GameRuntimeData _gameRuntimeData;
        [SerializeField, ES3NonSerializable]
        private CameraControlScript _cameraControlScript;

        public async override Task StartSceneAsync(SceneLoadingData data)
        {
            Debug.Log("Starting scene");
            if (!_gameRuntimeData)
                _gameRuntimeData = FindObjectOfType<GameRuntimeData>();
            if (!_cameraControlScript)
                _cameraControlScript = FindObjectOfType<CameraControlScript>();
            string entryPointTag = data != null ? data.targetEntryPointTag : "";
            string cameraBoundingBoxTag = data != null ? data.cameraBoundingBoxTag : "";
            CreatureAsset mainCharAsset = _gameRuntimeData.MainCharacterAsset;
            List<CreatureAsset> activeParty = _gameRuntimeData.ActivePartyAssets;
            List<Vector3> partyPositions = _gameRuntimeData.ActivePartySavedPositions;
            if (partyPositions != null && partyPositions.Count > 0)
            {
                await InstantiateParty(partyPositions, mainCharAsset, activeParty);
                Vector3 cameraPosition = partyPositions[0];
                HandleCameraPositionAndBounds(cameraBoundingBoxTag, cameraPosition);
            }
            else
            {
                AreaEntryPoint entryPoint = FindAreaEntryPoint(entryPointTag);
                await InstantiateParty(entryPoint.transform.position, mainCharAsset, activeParty);
                Vector3 cameraPosition = entryPoint.transform.position;
                HandleCameraPositionAndBounds(cameraBoundingBoxTag, cameraPosition);
            }
            await Task.WhenAll(InitializeObjects(FindInterfaces.Find<IInitialized>()));
            Debug.Log("Finished initializing objects!");
            GameManager.CurrentState = GameState.Exploration;

            static List<Task> InitializeObjects(List<IInitialized> objectsToInitialize)
            {
                List<Task> initializationTasks = new();
                foreach (IInitialized initializable in objectsToInitialize)
                {
                    Debug.Log("Starting initialization for object " + initializable.ToString());
                    initializationTasks.Add(Task.Run(async () =>
                    {
                        await initializable.InitializeAsync();
                    }));
                }
                return initializationTasks;
            }
        }

        private void HandleCameraPositionAndBounds(string cameraBoundingBoxTag, Vector3 cameraPosition)
        {
            if (this.TryFindFirstGameObjectWithTag(cameraBoundingBoxTag, out GameObject boundingBoxGO))
                if (boundingBoxGO.TryGetComponent(out BoundingBox boundingBox))
                    _cameraControlScript.CurrentBoundingBox = boundingBox;
            _cameraControlScript.ForceToPosition(cameraPosition);
        }

        protected async Task InstantiateParty(Vector3 position, CreatureAsset mainChar, List<CreatureAsset> party)
        {
            List<Vector3> positions = new() { position, position, position, position, position, position };
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
                if (this.TryFindFirstGameObjectWithTag(tag, out GameObject result))
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
