using SunsetSystems.Entities.Interactable;
using SunsetSystems.Input.CameraControl;
using System.Threading.Tasks;
using UnityEngine;
using SunsetSystems.LevelUtility;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Persistence;

namespace SunsetSystems.Core.SceneLoading
{
    public class SceneTransition : SerializedMonoBehaviour, IInteractionHandler, ITransition
    {
        [SerializeField]
        private TransitionType _type;
        [SerializeField, ShowIf("@this._type == TransitionType.SceneTransition")]
        private SceneLoadingData sceneToLoad;
        [SerializeField, ShowIf("@this._type == TransitionType.InternalTransition")]
        private Waypoint _targetEntryPoint;
        [SerializeField, ShowIf("@this._type == TransitionType.InternalTransition")]
        private BoundingBox _targetBoundingBox;
        [SerializeField, ShowIf("@this._type == TransitionType.InternalTransition")]
        private CameraControlScript _cameraControlScript;

        public bool HandleInteraction(IActionPerformer interactee)
        {
            Debug.Log("Interacting with area transition!");
            switch (_type)
            {
                case TransitionType.SceneTransition:
                    break;
                case TransitionType.InternalTransition:
                    break;
            }
            return true;
        }

        public void MoveToScene(SceneLoadingData data)
        {
            SaveLoadManager.UpdateRuntimeDataCache();
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
        SceneTransition, InternalTransition
    }
}