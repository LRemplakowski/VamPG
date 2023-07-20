using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Interfaces;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Interfaces
{
    public interface ICreature : IEntity, ICombatant
    {
        Task PerformAction(EntityAction action);
        bool HasActionsInQueue();

        void ForceToPosition(Vector3 position);
    }
}
