using SunsetSystems.Loading;
using SunsetSystems.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Dialogue
{
    public static class HavenDialogueCommands
    {
        [YarnCommand("GetUpFromBedDesiree")]
        public static void GetUpFromBedDesiree()
        {
            if (Tagger.tags.TryGetValue(TagConstants.SCENE_LOGIC, out List<GameObject> found))
            {
                if (found != null && found.Count > 0)
                {
                    _ = found[0].GetComponent<HavenSceneLogic>().MovePCToPositionAfterDialogue();
                }
            }
        }
    }
}
