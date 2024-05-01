using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.Utils.Extensions
{
    public static class CollectionExtensions
    {
        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            int enumerableCount = enumerable.Count();
            if (enumerableCount <= 0)
                return default;
            int itemIndex = Random.Range(0, enumerableCount - 1);
            Debug.Log($"Got random item from collection {enumerable}! Item index: {itemIndex}");
            return enumerable.ElementAt(itemIndex);
        }
    }
}