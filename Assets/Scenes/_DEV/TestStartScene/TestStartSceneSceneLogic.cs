using SunsetSystems.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Loading
{
    internal class TestStartSceneSceneLogic : DefaultSceneLogic
    {
        public async override Task StartSceneAsync(SceneLoadingData data)
        {
            await base.StartSceneAsync(data);
            Debug.Log("TestStartScene");
        }
    }
}
