using System;
using System.Collections.Generic;
using Utils.Dialogue;
using UnityEngine;

public class DialogueExecution
{
    private static readonly DialogueExecution instance = new DialogueExecution();

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
}
