using System.Threading.Tasks;
using SunsetSystems.Combat;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Party;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Core
{
    public class EmbassyClubSceneLogic : DefaultSceneLogic
    {
        [SerializeField]
        private ICreature _anastasiaCompanion;
        [SerializeField]
        private IEncounter _embassyShootoutEncounter;

        protected override void Awake()
        {
            base.Awake();
            EmbassyClubDialogueCommands._sceneLogic = this;
        }

        public override Task StartSceneAsync()
        {
            return base.StartSceneAsync();
        }

        private void DoRecruitAnastasia()
        {
            PartyManager.Instance.RecruitCharacter(_anastasiaCompanion);
        }

        public static class EmbassyClubDialogueCommands
        {
            public static EmbassyClubSceneLogic _sceneLogic;

            [YarnCommand("RecruitAnastasia")]
            public static void RecruitAnastasia()
            {
                _sceneLogic.DoRecruitAnastasia();
            }

            [YarnCommand("StartEmbassyShootout")]
            public static void StartEmbassyShootout()
            {
                _sceneLogic._embassyShootoutEncounter.Begin();
            }
        }
    }
}
