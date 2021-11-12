using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transitions.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace Utils.Scenes
{
    internal static class SceneInitializer
    {
        internal static void InitializeSingletons()
        {
            GameObject.FindObjectsOfType<MonoBehaviour>(true).OfType<IInitializable>().ToList().ForEach(o => o.Initialize());
        }
    }
}
