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

    public static bool TryFindAllGameObjectsWithTag(this GameObject go, string tag, out List<GameObject> result)
    {
        bool found = Tagger.tags.TryGetValue(tag, out List<GameObject> value);
        result = value;
        return found;
    }

    public static List<T> FindAllComponentsWithTag<T>(this GameObject go, string tag) where T : Component
    {
        List<T> result = new();
        if (go.TryFindAllGameObjectsWithTag(tag, out List<GameObject> found))
        {
            foreach (GameObject f in found)
            {
                if (f.TryGetComponent<T>(out T component))
                    result.Add(component);
            }
        }
        return result;
    }

    public static bool TryFindFirstGameObjectWithTag(this GameObject go, string tag, out GameObject result)
    {
        bool found = false;
        result = null;
        if (Tagger.tags.TryGetValue(tag, out List<GameObject> value))
        {
            if (value.Count > 0)
                result = value[0];
            else
                return false;
            found = result != null;
        }
        return found;
    }

    public static T FindFirstComponentWithTag<T>(this GameObject go, string tag) where T : Component
    {
        T result = null;
        if (go.TryFindFirstGameObjectWithTag(tag, out GameObject found))
            result = found.GetComponent<T>();
        return result;
    }

    public static bool TryFindAllGameObjectsWithTag(this Component co, string tag, out List<GameObject> result)
    {
        bool found = Tagger.tags.TryGetValue(tag, out List<GameObject> value);
        result = value;
        return found;
    }

    public static List<T> FindAllComponentsWithTag<T>(this Component co, string tag) where T : Component
    {
        List<T> result = new();
        if (co.TryFindAllGameObjectsWithTag(tag, out List<GameObject> found))
        {
            foreach (GameObject f in found)
            {
                if (f.TryGetComponent<T>(out T component))
                    result.Add(component);
            }
        }
        return result;
    }

    public static bool TryFindFirstGameObjectWithTag(this Component co, string tag, out GameObject result)
    {
        bool found = false;
        result = null;
        if (Tagger.tags.TryGetValue(tag, out List<GameObject> value))
        {
            if (value.Count > 0)
                result = value[0];
            else
                return false;
            found = result != null;
        }
        return found;
    }

    public static T FindFirstComponentWithTag<T>(this Component co, string tag) where T : Component
    {
        T result = null;
        if (co.TryFindFirstGameObjectWithTag(tag, out GameObject found))
            result = found.GetComponent<T>();
        return result;
    }

    public static void DestroyChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.Equals(transform))
                continue;
            else
                UnityEngine.Object.Destroy(child.gameObject);
        }
    }

    public static void DestroyChildrenImmediate(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.Equals(transform))
                continue;
            else
                UnityEngine.Object.DestroyImmediate(child.gameObject);
        }
    }
}