using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Loading;
using SunsetSystems.Data;

namespace SunsetSystems.Dialogue
{
    public class DialogueExecution
    {
        private static readonly DialogueExecution instance = new DialogueExecution();

        private DialogueExecution()
        {

        }

        public static void Execute(string execution)
        {
            MethodInvoker.ParseMethod(execution, out string methodName, out List<object> args);
            MethodInvoker.InvokeExecutional(instance, methodName, args);
            Debug.Log(methodName + ", " + args);
        }

        public static void TestMethod(string message)
        {
            Debug.Log(message);
        }

        public static void NameTransition(string sceneName, string entryPoint, string boundingBox)
        {
            _ = SceneLoader.Instance.LoadGameScene(new NameLoadingData(sceneName, entryPoint, boundingBox));
        }

        public static void IndexTransition(int sceneIndex, string entryPoint, string boundingBox)
        {
            _ = SceneLoader.Instance.LoadGameScene(new IndexLoadingData(sceneIndex, entryPoint, boundingBox));
        }
    }
}
