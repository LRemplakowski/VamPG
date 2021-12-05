namespace Transitions
{
    using Entities.Interactable;
    using SunsetSystems.Journal;
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

        private JournalManager journal;

        private void Start()
        {
            if (!journal)
                journal = ReferenceManager.GetManager<JournalManager>();
        }

        public override void Interact()
        {
            Debug.Log("Interacting with area transition!");
            switch (type)
            {
                case TransitionType.index:
                    MoveToScene(new IndexTransition(SceneIndex, targetEntryPointTag, new Entities.Characters.CreatureAsset[0]));
                    break;
                case TransitionType.name:
                    MoveToScene(new NameTransition(SceneName, targetEntryPointTag, new Entities.Characters.CreatureAsset[0]));
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