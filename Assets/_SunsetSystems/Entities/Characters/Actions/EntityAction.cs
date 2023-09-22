using SunsetSystems.Entities.Characters.Actions.Conditions;
using SunsetSystems.Entities.Characters.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public abstract class EntityAction
    {
        /// <summary>
        /// Priority actions clear action queue upon assignment.
        /// </summary>
        public bool IsPriority { get; protected set; }
        public bool ActionFinished { get; protected set; } = false;
        protected ICreature Owner { get; }
        protected List<Condition> conditions = new List<Condition>();

        public EntityAction(ICreature owner)
        {
            Owner = owner;
            IsPriority = false;
        }

        public EntityAction(ICreature owner, bool isPriority) : this(owner)
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
        public virtual void Abort()
        {
            conditions.Clear();
            ActionFinished = true;
        }

        public virtual bool IsFinished()
        {
            if (conditions.Count == 0)
            {
                Debug.LogError("Aborting action, no conditions present");
                this.Abort();
                return true;
            }
            foreach (Condition c in conditions)
            {
                if (c.IsMet())
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }
            this.Abort();
            ActionFinished = true;
            return true;
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
