using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public abstract class EntityAction
    {
        /// <summary>
        /// Priority actions clear action queue upon assignment.
        /// </summary>
        [ShowInInspector]
        private string name => this.ToString();
        [field: ShowInInspector]
        public bool IsPriority { get; protected set; }
        public bool ActionFinished { get; protected set; } = false;
        public bool ActionCanceled { get; protected set; } = false;
        protected IActionPerformer Owner { get; }
        [ShowInInspector]
        protected List<Condition> conditions = new List<Condition>();

        public EntityAction(IActionPerformer owner)
        {
            Owner = owner;
            IsPriority = false;
        }

        public EntityAction(IActionPerformer owner, bool isPriority) : this(owner)
        {
            IsPriority = isPriority;
        }

        /// <summary>
        /// Start executing action logic.
        /// </summary>
        public abstract void Begin();

        /// <summary>
        /// Do any relevant cleanup.
        /// </summary>
        public virtual void Cleanup()
        {
            conditions.Clear();
            ActionFinished = true;
        }

        public virtual void Abort()
        {
            conditions.Clear();
            ActionCanceled = true;
        }

        public virtual bool EvaluateAction()
        {
            if (ActionFinished || ActionCanceled)
                return true;
            if (conditions.Count == 0)
            {
                Debug.LogError("Aborting action, no conditions present");
                Abort();
                return true;
            }
            ActionFinished = conditions.All(c => c.IsMet());
            if (ActionFinished)
                Cleanup();
            return ActionFinished;
        }

        public override string ToString()
        {
            string action = "EntityAction: " + this.GetType() + "\nConditions:";
            foreach (Condition c in conditions)
            {
                action += "\n" + c;
            }
            return action;
        }
    } 
}
