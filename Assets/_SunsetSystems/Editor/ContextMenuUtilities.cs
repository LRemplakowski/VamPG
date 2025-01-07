using UnityEditor;
using UnityEngine;

namespace SunsetSystems.Utils.Editor 
{ 
    public static class ContextMenuUtilities
    {
        [MenuItem("CONTEXT/Component/Destroy All Components")] 
        public static void DestroyAllComponentsExceptTransform(MenuCommand command) 
        {
            if (command.context is Component component)
            {
                var gameObject = component.gameObject;
                var components = component.GetComponents<Component>();
                foreach (var comp in components)
                {
                    if (comp is not Transform)
                    {
                        Undo.DestroyObjectImmediate(comp);
                    }
                }
                EditorUtility.SetDirty(gameObject);
            }
        }
    }
}