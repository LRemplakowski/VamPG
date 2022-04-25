namespace SunsetSystems.Loading
{
    using Entities.Interactable;
    using SunsetSystems.Data;
    using SunsetSystems.Loading;
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
        private TransitionType _type;
        [SerializeField]
        private string _targetEntryPointTag;

        private SceneLoader _sceneLoader;
        private FadeScreenAnimator _fadeScreenAnimator;

        private void Start()
        {
            _sceneLoader = FindObjectOfType<SceneLoader>();
            _fadeScreenAnimator = FindObjectOfType<FadeScreenAnimator>();
        }

        public override void Interact()
        {
            Debug.Log("Interacting with area transition!");
            switch (_type)
            {
                case TransitionType.index:
                    MoveToScene(new IndexLoadingData(SceneIndex, _targetEntryPointTag));
                    break;
                case TransitionType.name:
                    MoveToScene(new NameLoadingData(SceneName, _targetEntryPointTag));
                    break;
            }
            base.Interact();
        }

        public async void MoveToScene(SceneLoadingData data)
        {
            await _fadeScreenAnimator.FadeOut(.5f);
            _ = _sceneLoader.LoadGameScene(data);
            _ = _fadeScreenAnimator.FadeIn(.5f);
        }
    }

    public enum TransitionType
    {
        index, name
    }
}