using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SunsetSystems.Loading
{
    internal class TestStartSceneSceneLogic : DefaultSceneLogic
    {
        public async override Task StartSceneAsync()
        {
            await base.StartSceneAsync();
            Debug.Log("TestStartScene");
        }
    }
}
