using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UMA.CharacterSystem;
using UnityEngine;

namespace SunsetSystems.Utils
{
    public class UMAUtility
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem(itemName: "UMA/Utility/Hide All Editor UMAs")]
        public static void HideAllEditorUMAs()
        {
            var dynamicAvatars = GameObject.FindObjectsByType<DynamicCharacterAvatar>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            dynamicAvatars.ForEach(avatar => avatar.editorTimeGeneration = false);
        }

        [UnityEditor.MenuItem(itemName: "UMA/Utility/Show All Editor UMAs")]
        public static void ShowAllEditorUMAs()
        {
            var dynamicAvatars = GameObject.FindObjectsByType<DynamicCharacterAvatar>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            dynamicAvatars.ForEach(avatar => avatar.editorTimeGeneration = true);
        }
#endif
    }
}
