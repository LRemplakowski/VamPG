namespace SunsetSystems.Loading
{
    using Entities.Characters;
    using Entities.Interactable;
    using SunsetSystems.Data;
    using SunsetSystems.Input.CameraControl;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;

    public class SceneTransition : InteractableEntity, ITransition
    {
        [SerializeField]
        private string _sceneName;

        [SerializeField]
        private int _sceneIndex;

        [SerializeField]
        private TransitionType _type;
        [SerializeField]
        private string _targetEntryPointTag;
        [SerializeField]
        private string _targetBoundingBoxTag;
        [SerializeField]
        private AreaEntryPoint _targetEntryPoint;
        [SerializeField]
        private BoundingBox _targetBoundingBox;
        [SerializeField]
        private SceneLoader _sceneLoader;
        [SerializeField]
        private SceneLoadingUIManager _fadeUI;
        [SerializeField]
        private CameraControlScript _cameraControlScript;

        private void Start()
        {
            _sceneLoader = FindObjectOfType<SceneLoader>();
            _fadeUI = this.FindFirstWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
        }

        public override void Interact()
        {
            Debug.Log("Interacting with area transition!");
            switch (_type)
            {
                case TransitionType.indexTransition:
                    MoveToScene(new IndexLoadingData(_sceneIndex, _targetEntryPointTag, _targetBoundingBoxTag));
                    break;
                case TransitionType.nameTransition:
                    MoveToScene(new NameLoadingData(_sceneName, _targetEntryPointTag, _targetBoundingBoxTag));
                    break;
                case TransitionType.internalTransition:
                    MoveToArea();
                    break;
            }
            base.Interact();
        }

        public void MoveToScene(SceneLoadingData data)
        {
            Task.Run(async () =>
            {
                await _sceneLoader.LoadGameScene(data);
            });
        }

        public async void MoveToArea()
        {
            await _fadeUI.DoFadeOutAsync(.5f);
            List<Creature> party = GameRuntimeData.GetActivePartyCreatures();
            List<Vector3> positions = PlayerInputHandler.GetPositionsFromPoint(_targetEntryPoint.transform.position);
            for (int i = 0; i < party.Count; i++)
            {
                party[i].ClearAllActions();
                party[i].ForceCreatureToPosition(positions[i]);
                await Task.Yield();
            }
            if (!_cameraControlScript)
                _cameraControlScript = FindObjectOfType<CameraControlScript>();
            _cameraControlScript.CurrentBoundingBox = _targetBoundingBox;
            _cameraControlScript.ForceToPosition(_targetEntryPoint.transform.position);
            await Task.Delay(500);
            await _fadeUI.DoFadeInAsync(.5f);
        }
    }

    public enum TransitionType
    {
        indexTransition, nameTransition, internalTransition
    }
}