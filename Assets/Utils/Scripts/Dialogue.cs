using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Dialogue
{
    public static class MethodInvoker
    {
        public static void InvokeExecutional(object executor, string methodName, List<object> args)
        {
            try
            {
                System.Type executorType = executor.GetType();
                System.Reflection.MethodInfo methodInfo = executorType.GetMethod(methodName);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(executor, args.ToArray());
                }
                else
                {
                    Debug.LogError("Invalid method name in Execution Node!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void InvokeConditional(object evaluator, string methodName, List<object> args, out bool returnValue)
        {
            try
            {
                System.Type evaluatorType = evaluator.GetType();
                System.Reflection.MethodInfo methodInfo = evaluatorType.GetMethod(methodName);
                if (methodInfo != null)
                {
                    var methodReturn = methodInfo.Invoke(evaluator, args.ToArray());
                    returnValue = (bool)methodReturn;
                }
                else
                {
                    Debug.LogError("Invalid method name in Condition Node!");
                    returnValue = false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                returnValue = false;
            }
        }

        public static void ParseMethod(string condition, out string methodName, out List<object> args)
        {
            string[] split = condition.Split('.');
            methodName = "";
            args = new List<object>();
            try
            {
                methodName = split[0];
                for (int i = 1; i < split.Length; i++)
                {
                    args.Add(ObjectParser.ParseObject(split[i]));
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    public static class ObjectParser
    {
        public static object ParseObject(string s)
        {
            if (int.TryParse(s, out int i))
                return i;
            if (double.TryParse(s, out double d))
                return d;
            if (bool.TryParse(s, out bool b))
                return b;
            return s;
        }
    }
}
