using System;
using UnityEngine.Events;

namespace SunsetSystems.Abilities
{
    public interface IExecutionConfirmationUI
    {
        void UpdateShowInterface(bool visible, Func<bool> interactableDelegate);

        void RegisterConfirmationCallback(UnityAction callback);
        void UnregisterConfirmationCallback(UnityAction callback);
    }
}