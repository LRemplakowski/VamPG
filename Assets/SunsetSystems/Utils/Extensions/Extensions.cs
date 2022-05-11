using SunsetSystems.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static bool IsNullOrEmpty<T>(this T[] array)
    {
        if (array == null || array.Length == 0)
            return true;
        else
            return Array.Exists(array, element => element != null);
    }

    public static bool TryFindAllWithTag(this GameObject go, string tag, out List<GameObject> result)
    {
        bool found = Tagger.tags.TryGetValue(tag, out List<GameObject> value);
        result = value;
        return found;

    }

    public static bool TryFindFirstWithTag(this GameObject go, string tag, out GameObject result)
    {
        bool found = Tagger.tags.TryGetValue(tag, out List<GameObject> value);
        result = value[0];
        return found;
    }

    public static bool TryFindAllWithTag(this Component co, string tag, out List<GameObject> result)
    {
        bool found = Tagger.tags.TryGetValue(tag, out List<GameObject> value);
        result = value;
        return found;

    }

    public static bool TryFindFirstWithTag(this Component co, string tag, out GameObject result)
    {
        bool found = Tagger.tags.TryGetValue(tag, out List<GameObject> value);
        result = value[0];
        return found;
    }

    public static void DestroyChildren(this Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            UnityEngine.Object.Destroy(transform.GetChild(i));
        }
    }
}