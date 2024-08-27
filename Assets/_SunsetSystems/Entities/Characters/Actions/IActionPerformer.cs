using System.Threading.Tasks;
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
    }
}
