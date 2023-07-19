using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Interfaces
{
    public interface ICreature : ICombatant
    {
        new IEntityReferences References { get; }

        Task PerformAction(EntityAction action);
        bool HasActionsInQueue();
    }
}
