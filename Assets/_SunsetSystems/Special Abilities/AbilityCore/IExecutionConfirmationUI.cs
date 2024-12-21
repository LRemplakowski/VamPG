using System;
using UnityEngine.Events;

namespace SunsetSystems.Abilities
{
    public interface IExecutionConfirmationUI
    {
        void SetActive(bool active);
        void SetExectuionValidationDelegate(Func<bool> validationDelegate);

        void RegisterConfirmationCallback(UnityAction callback);
        void UnregisterConfirmationCallback(UnityAction callback);
    }
}