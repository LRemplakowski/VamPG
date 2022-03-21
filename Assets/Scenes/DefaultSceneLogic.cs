using Entities.Characters;
using SunsetSystems.Formation;
using SunsetSystems.GameData;
using SunsetSystems.Management;
using SunsetSystems.Party;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Scenes
{
    public class DefaultSceneLogic : AbstractSceneLogic
    {
        public override void StartScene()
        {
            Debug.Log("Starting scene");
            GameData gameData = FindObjectOfType<GameData>();
            CreatureAsset mainCharAsset = gameData.MainCharacterAsset;
            List<CreatureAsset> activeParty = gameData.ActivePartyAssets;
            List<Vector3> partyPositions = gameData.ActivePartySavedPositions;
            AreaEntryPoint entryPoint = FindAreaEntryPoint("");
            if (partyPositions != null && partyPositions.Count > 0)
                InitializeParty(partyPositions, mainCharAsset, activeParty);
            else
                InitializeParty(entryPoint.transform.position, mainCharAsset, activeParty);
        }

        protected void InitializeParty(Vector3 position, CreatureAsset mainChar)
        {
            List<Vector3> positions = new List<Vector3>
            {
                position
            };
            InitializeParty(positions, mainChar, new List<CreatureAsset>());
        }

        protected void InitializeParty(Vector3 position, CreatureAsset mainChar, List<CreatureAsset> party)
        {
            List<Vector3> positions = FormationController.GetPositionsFromPoint(position);
            InitializeParty(positions, mainChar, party);
        }

        protected void InitializeParty(List<Vector3> positions, CreatureAsset mainChar, List<CreatureAsset> party)
        {
            Debug.Log("Initializing party");
            CreatureData mainCharData = null;
            List<CreatureData> partyData = new List<CreatureData>();
            if (mainChar != null && !isPartyInitialized)
            {
                mainCharData = InitializePartyMember(mainChar, positions[0]);
                for (int i = 0; i < party.Count; i++)
                {
                    partyData.Add(InitializePartyMember(party[i], positions[i + 1]));
                }

                isPartyInitialized = true;
            }
            GameData gameData = FindObjectOfType<GameData>();
            gameData.MainCharacterData = mainCharData;
            gameData.ActivePartyData.Clear();
            gameData.ActivePartyData.AddRange(partyData);
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
                GameObject obj = null;
                try
                {
                    obj = GameObject.FindGameObjectWithTag(tag);
                }
                catch (UnityException e)
                {
                    Debug.LogException(e);
                }
                if (obj)
                    entryPoint = obj.GetComponent<AreaEntryPoint>();
            }
            if (entryPoint == null)
            {
                entryPoint = FindObjectOfType<AreaEntryPoint>();
            }
            return entryPoint;
        }
    }
}
