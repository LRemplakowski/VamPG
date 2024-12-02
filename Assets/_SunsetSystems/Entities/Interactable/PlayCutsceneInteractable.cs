using Sirenix.OdinInspector;
using SunsetSystems.ActionSystem;
using UnityEngine;
using UnityEngine.Playables;

namespace SunsetSystems.Entities.Interactable
{
    public class PlayCutsceneInteractable : SerializedMonoBehaviour, IInteractionHandler
    {
        [SerializeField]
        private PlayableDirector _director;

        public bool HandleInteraction(IActionPerformer interactee)
        {
            _director.Play();
            return true;
        }
    }
}
