﻿namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    public class InteractionComplete : Condition
    {
        readonly IInteractable target;
        public InteractionComplete(IInteractable target)
        {
            this.target = target;
        }

        public override bool IsMet()
        {
            if (target.Interacted == true)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            string interaction = "Type<InteractionComplete>:\n" +
                "Interaction target: " + target + "\n" +
                "Interacted with target? " + target.Interacted;
            return interaction;
        }
    } 
}
