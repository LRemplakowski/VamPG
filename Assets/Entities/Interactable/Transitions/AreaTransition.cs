namespace Transitions
{
    using Entities.Interactable;
    using SunsetSystems.Data;
    using SunsetSystems.Management;
    using Transitions.Data;
    using Transitions.Manager;
    using UnityEngine;
    using SunsetSystems.Scenes;

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

        private SceneLoader sceneLoader;
        private FadeScreenAnimator fadeScreenAnimator;

        private void Start()
        {
            sceneLoader = FindObjectOfType<SceneLoader>();
            fadeScreenAnimator = FindObjectOfType<FadeScreenAnimator>();
        }

        public override void Interact()
        {
            Debug.Log("Interacting with area transition!");
            switch (type)
            {
                case TransitionType.index:
                    MoveToScene(new IndexLoadingData(SceneIndex, targetEntryPointTag));
                    break;
                case TransitionType.name:
                    MoveToScene(new NameLoadingData(SceneName, targetEntryPointTag));
                    break;
            }
            base.Interact();
        }

        public async void MoveToScene(SceneLoadingData data)
        {
            await fadeScreenAnimator.FadeOut(.5f);
            _ = sceneLoader.LoadGameScene(data);
            _ = fadeScreenAnimator.FadeIn(.5f);
        }
    }

    public enum TransitionType
    {
        index, name
    }
}