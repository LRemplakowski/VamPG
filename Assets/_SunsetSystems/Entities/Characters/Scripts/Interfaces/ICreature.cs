using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Creatures.Interfaces;
using SunsetSystems.Entities.Interfaces;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Interfaces
{
    public interface ICreature : IEntity, IActionPerformer, ICreatureTemplateProvider
    {
        new ICreatureReferences References { get; }

        void ForceToPosition(Vector3 position);
        void ForceToPosition(Transform positionTransform);
        void FacePointInSpace(Vector3 point);
        void FaceTransform(Transform transform);
        void InjectDataFromTemplate(ICreatureTemplate template);
    }
}
