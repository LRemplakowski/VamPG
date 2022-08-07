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

    public static List<T> FindAllComponentsWithTag<T>(this GameObject go, string tag) where T : Component
    {
        List<T> result = new();
        if (go.TryFindAllWithTag(tag, out List<GameObject> found))
        {
            foreach (GameObject f in found)
            {
                if (f.TryGetComponent<T>(out T component))
                    result.Add(component);
            }
        }
        return result;
    }

    public static bool TryFindFirstWithTag(this GameObject go, string tag, out GameObject result)
    {
        bool found = Tagger.tags.TryGetValue(tag, out List<GameObject> value);
        result = value?[0];
        return found;
    }

    public static T FindFirstComponentWithTag<T>(this GameObject go, string tag) where T : Component
    {
        T result = null;
        if (go.TryFindFirstWithTag(tag, out GameObject found))
            result = found.GetComponent<T>();
        return result;
    }

    public static bool TryFindAllWithTag(this Component co, string tag, out List<GameObject> result)
    {
        bool found = Tagger.tags.TryGetValue(tag, out List<GameObject> value);
        result = value;
        return found;
    }

    public static List<T> FindAllComponentsWithTag<T>(this Component co, string tag) where T : Component
    {
        List<T> result = new();
        if (co.TryFindAllWithTag(tag, out List<GameObject> found))
        {
            foreach (GameObject f in found)
            {
                if (f.TryGetComponent<T>(out T component))
                    result.Add(component);
            }
        }
        return result;
    }

    public static bool TryFindFirstWithTag(this Component co, string tag, out GameObject result)
    {
        bool found = Tagger.tags.TryGetValue(tag, out List<GameObject> value);
        result = value?[0];
        return found;
    }

    public static T FindFirstComponentWithTag<T>(this Component co, string tag) where T : Component
    {
        T result = null;
        if (co.TryFindFirstWithTag(tag, out GameObject found))
            result = found.GetComponent<T>();
        return result;
    }

    public static void DestroyChildren(this Transform transform)
    {
        for (int i = transform.childCount; i > 0; --i)
        {
            UnityEngine.Object.Destroy(transform.GetChild(i));
        }
    }
}