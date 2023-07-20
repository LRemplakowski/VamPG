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

    public override Task StartSceneAsync(LevelLoadingData data)
    {
        return base.StartSceneAsync(data);
    }

    private async Task DoRecruitAnastasia()
    {
        SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
        await fade.DoFadeOutAsync(.5f);
        anastasiaDialogue.gameObject.SetActive(false);
        anastasiaCompanion.References.GameObject.SetActive(true);
        PartyManager.RecruitCharacter(anastasiaCompanion);
        PartyManager.AddCreatureAsActivePartyMember(anastasiaCompanion);
        await new WaitForUpdate();
        await fade.DoFadeInAsync(.5f);
    }

    public static class EmbassyClubDialogueCommands
    {
        public static EmbassyClubSceneLogic sceneLogic;

        [YarnCommand("RecruitAnastasia")]
        public static void RecruitAnastasia()
        {
            _ = sceneLogic.DoRecruitAnastasia();
        }
    }
}
