using System.Threading.Tasks;
using SunsetSystems.Entities.Characters;
using UnityEngine;

namespace SunsetSystems.ActionSystem
{
    public interface IActionPerformer
    {
        Transform Transform { get; }
        MonoBehaviour CoroutineRunner { get; }
        ICreatureReferences References { get; }

        Task PerformAction(EntityAction action, bool clearQueue = false);
        EntityAction PeekCurrentAction { get;  }
        bool HasActionsQueued { get; }
    }
}
