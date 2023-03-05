using SunsetSystems.Data;
using SunsetSystems.Loading;
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

        public async override Task StartSceneAsync(LevelLoadingData data)
        {
            await base.StartSceneAsync(data);
        }
    }
}
