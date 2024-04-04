using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalagaan.HairDesignerExtension
{
    public class HairDesignerEvents
    {
        static Action<PlayModeStateChange> playStateChanged;
        static Action<PauseState> pauseStateChanged;

        public enum PlayModeStateChange
        {
            EnteredEditMode = 0,
            ExitingEditMode = 1,
            EnteredPlayMode = 2,
            ExitingPlayMode = 3
        }

        public enum PauseState
        {
            Paused = 0,
            Unpaused = 1
        }


        public static void RegisterPlayStateChanged(Action<PlayModeStateChange> action)
        {
            playStateChanged += action;
        }

        public static void RegisterPauseStateChanged(Action<PauseState> action)
        {
            pauseStateChanged += action;
        }


        public static void PlayStateChanged(PlayModeStateChange state )
        {
            //Debug.Log(state);
            if(playStateChanged!=null)
                playStateChanged(state);
        }


        public static void PauseStateChanged(PauseState state)
        {
            //Debug.Log(state);
            if (pauseStateChanged != null)
                pauseStateChanged(state);
        }

    }
}