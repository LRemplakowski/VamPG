using Redcode.Awaiting;
using SunsetSystems.Data;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Party;
using SunsetSystems.Persistence;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

public class EmbassyClubSceneLogic : DefaultSceneLogic
{
    [SerializeField]
    private ICreature anastasiaCompanion;
    [SerializeField]
    private Creature anastasiaDialogue;

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
        anastasiaDialogue.gameObject.SetActive(false);
        anastasiaCompanion.References.GameObject.SetActive(true);
        PartyManager.Instance.RecruitCharacter(anastasiaCompanion);
        PartyManager.Instance.AddCreatureAsActivePartyMember(anastasiaCompanion);
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
