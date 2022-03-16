using System;
using System.Collections.Generic;
using Utils.Dialogue;
using UnityEngine;
using Transitions.Manager;
using Transitions.Data;
using SunsetSystems.Management;

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

    public static void NameTransition(string sceneName)
    {
        UnityEngine.Object.FindObjectOfType<TransitionManager>().PerformTransition(new NameTransition(sceneName, ""));
    }

    public static void IndexTransition(int sceneIndex)
    {
        UnityEngine.Object.FindObjectOfType<TransitionManager>().PerformTransition(new IndexTransition(sceneIndex, ""));
    }
}
