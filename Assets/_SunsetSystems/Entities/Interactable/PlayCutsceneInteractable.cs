using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
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
