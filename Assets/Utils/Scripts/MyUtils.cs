using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyUtils
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

    public static class Roll
    {
        public class Outcome
        {
            public readonly int successes;
            public readonly bool isCritical, isMessy, isBestialFailure;
            public readonly List<int> normalDice = new List<int>(), hungerDice = new List<int>();

            public Outcome(int successes, bool isCritical, bool isMessy, bool isBestialFailure, List<int> normalDice, List<int> hungerDice)
            {
                this.successes = successes;
                this.isCritical = isCritical;
                this.isMessy = isMessy;
                this.isBestialFailure = isBestialFailure;
                this.normalDice = normalDice;
                this.hungerDice = hungerDice;
            }

            public Outcome(int successes, bool isCritical, List<int> normalDice)
            {
                this.successes = successes;
                this.isCritical = isCritical;
                isMessy = false;
                isBestialFailure = false;
                this.normalDice = normalDice;
            }
        }

        public static Outcome d10(int normalDice, int hungerDice)
        {
            System.Random r = new System.Random();
            int[] normals = new int[normalDice];
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = r.Next(1, 11);
            }
            int[] hunger = new int[hungerDice];
            for (int i = 0; i < hunger.Length; i++)
            {
                hunger[i] = r.Next(1, 11);
            }

            int successes = Array.FindAll(normals, n => n >= 6).Length + Array.FindAll(hunger, h => h >= 6).Length;
            int tens = Array.FindAll(normals, n => n == 10).Length;
            int hungerTens = Array.FindAll(hunger, h => h == 10).Length;
            bool isCritical = false;
            if (tens + hungerTens >= 2)
            {
                successes += (tens + hungerTens) / 2;
                isCritical = true;
            }
            bool isMessy = isCritical && hungerTens > 0;
            Debug.Log("d10:"
                + "\nnormal dice: " + DiceRollToString(normals)
                + "\nhunger dice: " + DiceRollToString(hunger)
                + "\nsuccesses: " + successes 
                + "\ntens: " + tens 
                + "\nhungerTens: " + hungerTens 
                + "\nisCritical? " + isCritical 
                + "\nisMessy?" + isMessy);
            return new Outcome(successes, isCritical, isMessy, false, new List<int>(normals), new List<int>(hunger));
        }

        public static Outcome d10(int normalDice, int hungerDice, int dc)
        {
            System.Random r = new System.Random();
            int[] normals = new int[normalDice];
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = r.Next(1, 11);
            }
            int[] hunger = new int[hungerDice];
            for (int i = 0; i < hunger.Length; i++)
            {
                hunger[i] = r.Next(1, 11);
            }

            int successes = Array.FindAll(normals, n => n >= 6).Length + Array.FindAll(hunger, h => h >= 6).Length;
            int tens = Array.FindAll(normals, n => n == 10).Length;
            int hungerOnes = Array.FindAll(hunger, h => h == 1).Length;
            int hungerTens = Array.FindAll(hunger, h => h == 10).Length;
            bool isCritical = false;
            if (tens + hungerTens >= 2)
            {
                successes += (tens + hungerTens) / 2;
                isCritical = true;
            }
            bool isMessy = isCritical && hungerTens > 0;
            bool isBestial = hungerOnes > 0 && successes < dc;
            Debug.Log("d10:"
                + "\nnormal dice: " + DiceRollToString(normals)
                + "\nhunger dice: " + DiceRollToString(hunger)
                + "\nsuccesses: " + successes
                + "\nDC: " + dc
                + "\ntens: " + tens
                + "\nhungerTens: " + hungerTens
                + "\nisCritical? " + isCritical
                + "\nisMessy?" + isMessy
                + "\nisBestial? " + isBestial);
            return new Outcome(successes, isCritical, isMessy, isBestial, new List<int>(normals), new List<int>(hunger));
        }

        public static Outcome d10(int dice)
        {
            System.Random r = new System.Random();
            int[] roll = new int[dice];
            for (int i = 0; i < roll.Length; i++)
            {
                roll[i] = r.Next(1, 11);
            }

            int successes = Array.FindAll(roll, n => n >= 6).Length;
            int tens = Array.FindAll(roll, n => n == 10).Length;
            bool isCritical = false;
            if (tens >= 2)
            {
                successes += tens / 2;
                isCritical = true;
            }
            Debug.Log("d10:"
                + "\ndice: " + DiceRollToString(roll)
                + "\nsuccesses: " + successes
                + "\ntens: " + tens
                + "\nisCritical? " + isCritical);
            return new Outcome(successes, isCritical, new List<int>(roll));
        }

        private static string DiceRollToString(int[] roll)
        {
            string result = "";
            foreach(int i in roll)
            {
                result = result + i + ", ";
            }
            return result;
        }
    }
}
