using SunsetSystems.Journal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transitions.Data;
using Transitions.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singleton;

namespace Utils.Scenes
{
    internal static class SceneInitializer
    {
        internal static void InitializeSingletons()
        {
            UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(true).OfType<IInitializable>().ToList().ForEach(o => o.Initialize());
        }

        internal static void InitializePlayableCharacters()
        {
            AreaEntryPoint entryPoint = null;
            TransitionData data = SceneLoader.CachedTransitionData;
            string tag = data != null ? data.targetEntryPointTag : "";
            if (!tag.Equals(""))
            {
                GameObject obj = null;
                try
                {
                    obj = GameObject.FindGameObjectWithTag(tag);
                }
                catch (UnityException e)
                {
                    Debug.LogException(e);
                }
                if (obj)
                    entryPoint = obj.GetComponent<AreaEntryPoint>();
            }
            if (entryPoint == null)
            {
                entryPoint = UnityEngine.Object.FindObjectOfType<AreaEntryPoint>();
            }

            GameJournal journal = UnityEngine.Object.FindObjectOfType<GameJournal>();
            if (journal)
                journal.InitializeParty(entryPoint != null ? entryPoint.transform.position : Vector3.zero);
        }
    }
}
