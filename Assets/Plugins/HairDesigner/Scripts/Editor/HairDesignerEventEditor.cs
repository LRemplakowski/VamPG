using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Kalagaan.HairDesignerExtension
{
    [InitializeOnLoadAttribute]
    public class HairDesignerEventEditor
    {
        static HairDesignerEventEditor()
        {
            EditorApplication.playModeStateChanged += PlayStateChanged;
            EditorApplication.pauseStateChanged += PauseStateChanged;
            //EditorApplication.update += Update;
        }

        private static void PlayStateChanged(PlayModeStateChange state)
        {            
            HairDesignerEvents.PlayStateChanged((HairDesignerEvents.PlayModeStateChange)state);
        }

        private static void PauseStateChanged(PauseState state)
        {
            HairDesignerEvents.PauseStateChanged((HairDesignerEvents.PauseState)state);
        }

        static void Update()
        {

            /*
            if(EditorApplication.isCompiling)
                Debug.Log("isCompiling");
            else
                Debug.Log("is not Compiling");
                */
        }
    }
}