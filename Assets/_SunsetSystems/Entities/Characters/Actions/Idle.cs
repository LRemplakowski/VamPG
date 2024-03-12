using SunsetSystems.Entities.Characters.Interfaces;

namespace SunsetSystems.Entities.Characters.Actions
{
    [System.Serializable]
    public class Idle : EntityAction
    {
        public Idle(IActionPerformer owner) : base(owner) { }

        public override void Cleanup()
        {

        }

        public override void Begin()
        {

        }

        public override bool EvaluateAction()
        {
            return false;
        }
    } 
}
