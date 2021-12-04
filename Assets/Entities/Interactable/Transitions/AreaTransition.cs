namespace Transitions
{
    using Entities.Interactable;
    using SunsetSystems.Management;
    using Transitions.Data;
    using Transitions.Manager;
    using UnityEngine;

    public class AreaTransition : InteractableEntity, ITransition
    {
        [SerializeField]
        private string _sceneName;
        public string SceneName
        {
            get => _sceneName;
            private set => _sceneName = value;
        }

        [SerializeField]
        private int _sceneIndex;
        public int SceneIndex
        {
            get => _sceneIndex;
            private set => _sceneIndex = value;
        }

        [SerializeField]
        private TransitionType type;

        public override void Interact()
        {
            Debug.Log("Interacting with area transition!");
            switch (type)
            {
                case TransitionType.index:
                    MoveToScene(new IndexTransition(SceneIndex));
                    break;
                case TransitionType.name:
                    MoveToScene(new NameTransition(SceneName));
                    break;
            }
            base.Interact();
        }

        public void MoveToScene(TransitionData data)
        {
            ReferenceManager.GetManager<TransitionManager>().PerformTransition(data);
        }
    }

    public enum TransitionType
    {
        index, name
    }
}