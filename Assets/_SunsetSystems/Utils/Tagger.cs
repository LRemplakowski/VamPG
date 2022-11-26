using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public class Tagger : MonoBehaviour
    {
        public static Dictionary<string, List<GameObject>> tags = new();
        new public string tag = "";

        private void Awake()
        {
            if (!tags.ContainsKey(tag))
                tags[tag] = new List<GameObject>();
            tags[tag].Add(gameObject);
        }

        private void OnDestroy()
        {
            tags[tag].Remove(gameObject);
        }
    }
}
