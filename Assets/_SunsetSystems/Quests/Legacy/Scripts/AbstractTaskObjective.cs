using System;
using UnityEngine;

namespace SunsetSystems.Quests.Tasks
{
    [Serializable]
    public abstract class AbstractTaskObjective : ScriptableObject, ITaskObjective
    {
        public abstract bool IsFinished();
    }
}
