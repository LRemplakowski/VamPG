using Entities.Characters;
using SunsetSystems.Formation;
using SunsetSystems.GameData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Scenes
{
    public class DefaultSceneLogic : AbstractSceneLogic
    {

        public override void StartScene()
        {
            GameData gameData = FindObjectOfType<GameData>();
            CreatureAsset mainCharAsset = gameData.MainCharacterAsset;
            List<CreatureAsset> activeParty = gameData.ActivePartyAssets;
            AreaEntryPoint entryPoint = FindAreaEntryPoint("");
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
            gameData.MainCharDataComponent = mainCharData;
            gameData.ActivePartyDataComponents.Clear();
            gameData.ActivePartyDataComponents.AddRange(partyData);

        }

        protected CreatureData InitializePartyMember(CreatureAsset asset, Vector3 position)
        {
            CreatureData creature = Instantiate(creaturePrefab, position, Quaternion.identity);
            creature.SetData(asset);
            creature.CreateCreature();
            return creature;
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
