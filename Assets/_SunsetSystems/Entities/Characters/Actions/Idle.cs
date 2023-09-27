using SunsetSystems.Entities.Characters.Interfaces;

namespace SunsetSystems.Entities.Characters.Actions
{
    public class Idle : EntityAction
    {
        public Idle(IActionPerformer owner) : base(owner) { }

        public override void Abort()
        {

        }

        public override void Begin()
        {

        }

        public override bool EvaluateActionFinished()
        {
            return false;
        }
    } 
}
