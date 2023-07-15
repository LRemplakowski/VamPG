using Entities.Characters.Data;
using SunsetSystems.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace UI.CharacterPortraits
{
    [RequireComponent(typeof(Tagger))]
    public class PartyPortraitsController : MonoBehaviour
    {
        [SerializeField]
        private GameObject portraitPrefab;
        private readonly List<PortraitController> portraits = new();

        internal void AddPortrait(Sprite portrait)
        {
            GameObject portraitGO = Instantiate(portraitPrefab, this.transform);
            //portraitGO.name = data.name + " Portrait";
            PortraitController portraitController = portraitGO.GetComponent<PortraitController>();
            if (portraitController == null)
                Debug.LogWarning("jebany null");
            portraitController.InitPotrait(portrait);
            portraits.Add(portraitController);
        }

        internal void Clear()
        {
            portraits.ForEach(p => Destroy(p.gameObject));
            portraits.Clear();
        }
    }
}
