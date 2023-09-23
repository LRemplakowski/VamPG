using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    public interface IActionPerformer
    {
        Transform Transform { get; }

        Task PerformAction(EntityAction action, bool clearQueue = false);

        T GetComponent<T>() where T : Component;
        T GetComponentInChildren<T>() where T : Component;
    }
}
