using System;
using System.Collections.Generic;
using UnityEngine;
using MyUtils;

public class DialogueCondition
{
    private static readonly DialogueCondition instance = new DialogueCondition();

    public static bool Evaluate(string condition)
    {
        MethodInvoker.ParseMethod(condition, out string methodName, out List<object> args);
        MethodInvoker.InvokeConditional(instance, methodName, args, out bool result);
        Debug.Log(methodName + ", " + args + ", " + result);
        return result;
    }

    public static bool PlayerSex(string sex)
    {
        if (Enum.TryParse(sex, out Creature.Sex result))
        {
            return GameManager.GetPlayer().GetSex().Equals(result);
        }
        else
            return false;
    }
}
