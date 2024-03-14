using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Party;
using SunsetSystems.Persistence;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Core
{
    public class EmbassyClubSceneLogic : DefaultSceneLogic
    {
        [SerializeField]
        private ICreature anastasiaCompanion;

        protected override void Awake()
        {
            base.Awake();
            EmbassyClubDialogueCommands.sceneLogic = this;
        }

        public override Task StartSceneAsync()
        {
            return base.StartSceneAsync();
        }

        private void DoRecruitAnastasia()
        {
            PartyManager.Instance.RecruitCharacter(anastasiaCompanion);
        }

        public static class EmbassyClubDialogueCommands
        {
            public static EmbassyClubSceneLogic sceneLogic;

            [YarnCommand("RecruitAnastasia")]
            public static void RecruitAnastasia()
            {
                sceneLogic.DoRecruitAnastasia();
            }
        }
    }
}
