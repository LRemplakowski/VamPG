using SunsetSystems.Data;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Persistence
{
    internal class TestStartSceneSceneLogic : DefaultSceneLogic
    {
        public async override Task StartSceneAsync(LevelLoadingData data)
        {
            await base.StartSceneAsync(data);
        }
    }
}
