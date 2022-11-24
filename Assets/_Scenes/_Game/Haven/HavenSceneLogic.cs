using Redcode.Awaiting;
using SunsetSystems.Data;
using SunsetSystems.Dialogue;
using SunsetSystems.Party;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Loading
{
    public class HavenSceneLogic : DefaultSceneLogic
    {
        [SerializeField, ES3NonSerializable]
        private YarnProject _sceneDialogues;
        [SerializeField, ES3NonSerializable]
        private string _wakeUpStartNode;
        [SerializeField, ES3NonSerializable]
        private Transform _startPosition;
        [SerializeField]
        private GameObject _desireeOnBed;

        public async override Task StartSceneAsync(SceneLoadingData data)
        {
            await base.StartSceneAsync(data);
            await new WaitForUpdate();
            PartyManager.MainCharacter.Agent.Warp(new Vector3(100, 100, 100));
            await new WaitForSeconds(2);
            DialogueManager.StartDialogue(_wakeUpStartNode, _sceneDialogues);
        }

        public async Task MovePCToPositionAfterDialogue()
        {
            SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
            await fade.DoFadeOutAsync(.5f);
            await new WaitForUpdate();
            _desireeOnBed.SetActive(false);
            PartyManager.MainCharacter.Agent.Warp(_startPosition.position);
            await new WaitForUpdate();
            await fade.DoFadeInAsync(.5f);
        }
    }
}
