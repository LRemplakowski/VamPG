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
        private List<PortraitController> portraits = new List<PortraitController>();

        internal void AddPortrait(CreatureUIData data)
        {
            GameObject portraitGO = Instantiate(portraitPrefab, this.transform);
            portraitGO.name = data.name + " Portrait";
            PortraitController portraitController = portraitGO.GetComponent<PortraitController>();
            if (portraitController == null)
                Debug.LogWarning("jebany null");
            portraitController.InitPotrait(data);
            portraits.Add(portraitController);
        }

        internal void Clear()
        {
            portraits.ForEach(p => Destroy(p.gameObject));
            portraits.Clear();
        }
    }
}
