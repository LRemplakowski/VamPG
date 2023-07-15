using Redcode.Awaiting;
using SunsetSystems.Data;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Party;
using SunsetSystems.Persistence;
using System.Threading.Tasks;
using UnityEngine;

public class EmbassyClubSceneLogic : DefaultSceneLogic
{
    [SerializeField]
    private PlayerControlledCharacter anastasiaCompanion;
    [SerializeField]
    private Creature anastasiaDialogue;

    protected override void Awake()
    {
        base.Awake();
        EmbassyClubDialogueCommands.sceneLogic = this;
    }

    public override Task StartSceneAsync(LevelLoadingData data)
    {
        return base.StartSceneAsync(data);
    }

    private async Task DoRecruitAnastasia()
    {
        SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
        await fade.DoFadeOutAsync(.5f);
        anastasiaDialogue.gameObject.SetActive(false);
        anastasiaCompanion.gameObject.SetActive(true);
        PartyManager.RecruitCharacter(anastasiaCompanion.Data);
        PartyManager.AddCreatureAsActivePartyMember(anastasiaCompanion);
        await new WaitForUpdate();
        await fade.DoFadeInAsync(.5f);
    }

    public static class EmbassyClubDialogueCommands
    {
        public static EmbassyClubSceneLogic sceneLogic;
        public static void RecruitAnastasia()
        {
            _ = sceneLogic.DoRecruitAnastasia();
        }
    }
}
