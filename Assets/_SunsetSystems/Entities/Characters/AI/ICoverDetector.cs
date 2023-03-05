using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Cover;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ICoverDetector
    {
        bool IsPositionNearCover(GridElement gridPos, out List<Cover> coverSources);
        bool FiringLineObstructedByCover(Creature attacker, Creature target, out Cover coverSource);
    }
}
