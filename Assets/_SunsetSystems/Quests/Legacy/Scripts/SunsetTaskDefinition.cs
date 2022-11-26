using CleverCrow.Fluid.QuestJournals;
using CleverCrow.Fluid.QuestJournals.Tasks;
using SunsetSystems.Quests.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Quests
{
    [CreateMenu("Sunset Task")]
    public class SunsetTaskDefinition : TaskDefinitionBase
    {
        [SerializeField]
        private List<AbstractTaskObjective> _completionObjectives = new();
        public List<ITaskObjective> CompletionObjectives => new(_completionObjectives);
    }
}
