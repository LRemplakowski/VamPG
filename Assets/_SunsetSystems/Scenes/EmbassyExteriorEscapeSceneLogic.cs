using SunsetSystems.Entities.Characters;
using SunsetSystems.Party;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Core
{
    public class EmbassyExteriorEscapeSceneLogic : DefaultSceneLogic
    {
        [SerializeField]
        private ICreature _sforza;

        protected override void Awake()
        {
            base.Awake();
            EmbassyExteriorEscaleDialogueCommands.SceneLogic = this;
        }

        public static class EmbassyExteriorEscaleDialogueCommands
        {
            public static EmbassyExteriorEscapeSceneLogic SceneLogic;

            [YarnCommand("RecruitCaterina")]
            public static void RecruitCaterina()
            {
                PartyManager.Instance.RecruitCharacter(SceneLogic._sforza);
            }
        }
    }
}
