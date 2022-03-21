using System;
using System.Collections.Generic;
using Utils.Dialogue;
using UnityEngine;
using Transitions.Manager;
using Transitions.Data;
using SunsetSystems.Management;
using Utils.Scenes;

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

    public async static void NameTransition(string sceneName)
    {
        FadeScreenAnimator fadeScreenAnimator = UnityEngine.Object.FindObjectOfType<FadeScreenAnimator>(true);
        SceneLoader sceneLoader = UnityEngine.Object.FindObjectOfType<SceneLoader>();
        await fadeScreenAnimator.FadeOut(.5f);
        _ = sceneLoader.LoadGameScene(new NameLoadingData(sceneName, ""));
        _ = fadeScreenAnimator.FadeIn(.5f);
    }

    public async static void IndexTransition(int sceneIndex)
    {
        FadeScreenAnimator fadeScreenAnimator = UnityEngine.Object.FindObjectOfType<FadeScreenAnimator>(true);
        SceneLoader sceneLoader = UnityEngine.Object.FindObjectOfType<SceneLoader>();
        await fadeScreenAnimator.FadeOut(.5f);
        _ = sceneLoader.LoadGameScene(new IndexLoadingData(sceneIndex, ""));
        _ = fadeScreenAnimator.FadeIn(.5f);
    }
}
