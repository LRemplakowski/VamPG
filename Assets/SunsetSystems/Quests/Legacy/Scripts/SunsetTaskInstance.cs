using CleverCrow.Fluid.QuestJournals.Tasks;
using SunsetSystems.Quests.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Quests
{
    public class SunsetTaskInstance : TaskInstance
    {
        public delegate void TaskCompletedHandler(ITaskInstance taskDefinition);
        public static event TaskCompletedHandler TaskCompleted;

        private List<ITaskObjective> completionObjectives;

        public SunsetTaskInstance(SunsetTaskDefinition definition) : base(definition)
        {
            this.completionObjectives = definition.CompletionObjectives;
        }

        public void CheckIfTaskIsFinished()
        {
            if (completionObjectives.TrueForAll(objective => objective.IsFinished()) && this.Status.Equals(TaskStatus.Ongoing))
            {
                this.Complete();
                TaskCompleted?.Invoke(this);
            }
        }
    }
}
