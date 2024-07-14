using SunsetSystems.Entities.Characters.Navigation;
using UnityEngine;

namespace SunsetSystems.Entities.Characters.Actions.Conditions
{
    [System.Serializable]
    public class Destination : Condition
    {
        [SerializeField]
        private INavigationManager agent;

        public Destination(INavigationManager agent)
        {
            this.agent = agent;
        }
        public override bool IsMet()
        {
            return agent.FinishedCurrentPath;
        }
    }
}
