using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Spellbook
{
    public interface IAbility
    {

    }
    
    public interface IRequiresTarget
    {
        IEnumerator<ITargetable> GetTarget();
    }

    public interface ITargetable
    {

    }
}
