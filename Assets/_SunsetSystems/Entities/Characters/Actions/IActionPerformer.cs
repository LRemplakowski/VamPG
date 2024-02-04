using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunsetSystems.Entities.Creatures.Interfaces;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    public interface IActionPerformer
    {
        Transform Transform { get; }
        MonoBehaviour CoroutineRunner { get; }
        ICreatureReferences References { get; }

        Task PerformAction(EntityAction action, bool clearQueue = false);
        EntityAction PeekCurrentAction { get;  }
        bool HasActionsQueued { get; }

        T GetComponent<T>() where T : Component;
        T GetComponentInChildren<T>() where T : Component;
    }
}
