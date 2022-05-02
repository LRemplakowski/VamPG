using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Threading.Tasks;

namespace Glitchers
{
    /// <summary>
    /// Author: Glitchers
    /// Source: https://gist.github.com/glitchersgames/c1d33a635fa0ca76e38de0591bb1b798
    /// </summary>
    public static class FindInterfaces
    {
        public static List<T> Find<T>()
        {
            var interfaces = Object.FindObjectsOfType<MonoBehaviour>().OfType<T>().ToList();
            Debug.Log("Found interfaces? " + interfaces != null);
            Debug.Log("Interfaces found: " + interfaces.Count);
            return interfaces;
        }
    }
}