using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interactable;
using UnityEngine;

namespace SunsetSystems.Economy
{
    public class MerchantTrigger : SerializedMonoBehaviour, IInteractionHandler
    {
        public bool HandleInteraction(IActionPerformer interactee)
        {
            throw new System.NotImplementedException();
        }
    }
}
