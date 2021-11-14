using Entities.Characters.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.CharacterPortraits
{
    public class PartyPortraitsController : MonoBehaviour
    {
        [SerializeField]
        private GameObject portraitPrefab;
        private List<PortraitController> portraits;

        public void AddPortrait(CreatureData data)
        {
            GameObject portraitGO = Instantiate(portraitPrefab, this.transform);
            portraitGO.name = data.name + " Portrait";
            PortraitController portraitController = portraitGO.GetComponent<PortraitController>();
            portraitController.InitPotrait(data);
            portraits.Add(portraitController);
        }
    }
}
