using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Creatures.Interfaces;
using SunsetSystems.Entities.Interfaces;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Interfaces
{
    public interface ICreature : IEntity, IActionPerformer
    {
        new ICreatureReferences References { get; }
        Task PerformAction(EntityAction action, bool clearQueue = false);
        bool HasActionsInQueue();

        void ForceToPosition(Vector3 position);

        void InjectDataFromTemplate(ICreatureTemplate template);
    }
}
