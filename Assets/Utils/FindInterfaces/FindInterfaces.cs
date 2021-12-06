using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            List<T> interfaces = new List<T>();
            GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var rootGameObject in rootGameObjects)
            {
                T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>();
                foreach (var childInterface in childrenInterfaces)
                {
                    interfaces.Add(childInterface);
                }
            }
            return interfaces;
        }
    }
}