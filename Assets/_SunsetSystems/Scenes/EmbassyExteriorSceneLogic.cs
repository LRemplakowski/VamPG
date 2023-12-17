using SunsetSystems.Data;
using SunsetSystems.Dialogue;
using SunsetSystems.Persistence;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems
{
    public class EmbassyExteriorSceneLogic : DefaultSceneLogic
    {
        [SerializeField]
        private YarnProject _sceneDialogueProject;
        [SerializeField]
        private string _sceneStartDialogueNode;

        private bool _firstVisit = true;

        public async override Task StartSceneAsync()
        {
            await base.StartSceneAsync();
            if (_firstVisit)
                DialogueManager.Instance.StartDialogue(_sceneStartDialogueNode, _sceneDialogueProject);
        }

        public override object GetSaveData()
        {
            EmbassyExteriorSaveData saveData = new();
            saveData.FirstVisit = _firstVisit;
            return saveData;
        }

        public override void InjectSaveData(object data)
        {
            EmbassyExteriorSaveData saveData = data as EmbassyExteriorSaveData;
            _firstVisit = saveData.FirstVisit;
        }

        private class EmbassyExteriorSaveData : SaveData
        {
            public bool FirstVisit;
        }
    }
}
