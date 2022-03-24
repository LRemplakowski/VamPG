using System;

public static class Extensions
{
    public static bool IsNullOrEmpty<T> (this T[] array)
    {
        if (array == null || array.Length == 0)
            return true;
        else
            return Array.Exists(array, element => element != null);
    }
}

