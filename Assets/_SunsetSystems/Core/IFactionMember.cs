using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Combat;
using UnityEngine;

namespace SunsetSystems.Entities
{
    public interface IFactionMember
    {
        Faction GetFaction();

        bool IsFriendlyTowards(IFactionMember other);
        bool IsHostileTowards(IFactionMember other);
        bool IsMe(IFactionMember other);
    }
}
