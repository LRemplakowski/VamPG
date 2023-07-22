﻿using SunsetSystems.Entities.Interactable;
using SunsetSystems.Data;
using SunsetSystems.Input.CameraControl;
using System.Threading.Tasks;
using UnityEngine;
using SunsetSystems.LevelUtility;

namespace SunsetSystems.Persistence
{
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
        private Waypoint _targetEntryPoint;
        [SerializeField]
        private BoundingBox _targetBoundingBox;
        [SerializeField]
        private CameraControlScript _cameraControlScript;

        private Task loadingTask = null;

        protected override void HandleInteraction()
        {
            if (loadingTask != null)
                return;
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
                    _ = MoveToArea();
                    break;
            }
        }

        public void MoveToScene(LevelLoadingData data)
        {
            //loadingTask = LevelLoader.Instance.LoadGameLevel(data);
        }

        public async Task MoveToArea()
        {
            //await _fadeUI.DoFadeOutAsync(.5f);
            //List<ICreature> party = PartyManager.ActiveParty;
            //for (int i = 0; i < party.Count; i++)
            //{
            //    party[i].ForceToPosition(_targetEntryPoint.transform.position);
            //    await Task.Yield();
            //}
            //if (!_cameraControlScript)
            //    _cameraControlScript = FindObjectOfType<CameraControlScript>();
            //_cameraControlScript.CurrentBoundingBox = _targetBoundingBox;
            //_cameraControlScript.ForceToPosition(_targetEntryPoint.transform.position);
            //await Task.Delay(500);
            //await _fadeUI.DoFadeInAsync(.5f);
        }
    }

    public enum TransitionType
    {
        indexTransition, nameTransition, internalTransition
    }
}