using SunsetSystems.Data;
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

        public async override Task StartSceneAsync(LevelLoadingData data)
        {
            await base.StartSceneAsync(data);
        }

        public override object GetSaveData()
        {
            return base.GetSaveData();
        }

        public override void InjectSaveData(object data)
        {
            base.InjectSaveData(data);
        }

        private class EmbassyExteriorSaveData : SaveData
        {

        }
    }
}
