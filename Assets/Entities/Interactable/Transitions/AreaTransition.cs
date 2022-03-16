namespace Transitions
{
    using Entities.Interactable;
    using SunsetSystems.GameData;
    using SunsetSystems.Management;
    using Transitions.Data;
    using Transitions.Manager;
    using UnityEngine;

    public class AreaTransition : InteractableEntity, ITransition
    {
        [SerializeField]
        private string _sceneName;
        public string SceneName { get => _sceneName; }

        [SerializeField]
        private int _sceneIndex;
        public int SceneIndex { get => _sceneIndex; }

        [SerializeField]
        private TransitionType type;
        [SerializeField]
        private string targetEntryPointTag;

        private TransitionManager transitionManager;

        private void Start()
        {
            transitionManager = FindObjectOfType<TransitionManager>();
        }

        public override void Interact()
        {
            Debug.Log("Interacting with area transition!");
            switch (type)
            {
                case TransitionType.index:
                    MoveToScene(new IndexTransition(SceneIndex, targetEntryPointTag));
                    break;
                case TransitionType.name:
                    MoveToScene(new NameTransition(SceneName, targetEntryPointTag));
                    break;
            }
            base.Interact();
        }

        public void MoveToScene(TransitionData data)
        {
            transitionManager.PerformTransition(data);
        }
    }

    public enum TransitionType
    {
        index, name
    }
}