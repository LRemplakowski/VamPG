using Sirenix.OdinInspector;
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
        [ShowInInspector]
        private readonly List<PortraitController> portraits = new();

        public void AddPortrait(Sprite portrait)
        {
            GameObject portraitGO = Instantiate(portraitPrefab, this.transform);
            PortraitController portraitController = portraitGO.GetComponent<PortraitController>();
            if (portraitController == null)
                Debug.LogWarning("jebany null");
            portraitController.InitPotrait(portrait);
            portraits.Add(portraitController);
        }

        public void Clear()
        {
            portraits.ForEach(p => Destroy(p.gameObject));
            portraits.Clear();
        }
    }
}
