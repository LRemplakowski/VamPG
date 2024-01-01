using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Characters.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Interactable
{
    public interface IInteractionHandler
    {
        bool HandleInteraction(IActionPerformer interactee);
    }
}
