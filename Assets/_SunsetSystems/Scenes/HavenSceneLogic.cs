using System.Collections.Generic;
using System.Threading.Tasks;
using Redcode.Awaiting;
using Sirenix.OdinInspector;
using SunsetSystems.Core.SceneLoading.UI;
using SunsetSystems.Dialogue;
using SunsetSystems.Dialogue.Interfaces;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Characters.Actions;
using SunsetSystems.Entities.Characters.Interfaces;
using SunsetSystems.Entities.Interactable;
using SunsetSystems.Game;
using SunsetSystems.Input.CameraControl;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.LevelUtility;
using SunsetSystems.Party;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Persistence
{
    public class HavenSceneLogic : DefaultSceneLogic
    {
        [SerializeField, ES3NonSerializable]
        private YarnProject _sceneDialogues;
        [SerializeField, ES3NonSerializable]
        private string _wakeUpStartNode;
        [SerializeField, ES3NonSerializable]
        private Transform _startPosition;
        [SerializeField]
        private Vector3 _cameraStartPoint, _cameraStartRotation;
        [Title("Prologue")]
        [SerializeField]
        private GameObject _desireeOnBed;
        [SerializeField]
        private GameObject _handgun;
        [SerializeField]
        private GameObject _crowbar, _gun;
        [SerializeField]
        private Weapon _handgunItem;
        [SerializeField]
        private IInteractable _kitchenSink;
        [SerializeField]
        private string landlordDialogueEntryNode;
        [SerializeField]
        private IDialogueSource _phone;
        [SerializeField]
        private ICreature _landlord;
        [SerializeField]
        private IDialogueSource _landlordDialogue;
        [SerializeField]
        private Waypoint _landlordSpawnWaypoint, _landlordSinkWaypoint, _pcLandlordVisitWaypoint, _pcLandlordSinkWaypoint;
        [SerializeField]
        private IInteractable _apartmentDoorLandlordInteraction;
        [SerializeField]
        private string _landlordVisitDialogueEntryNode;
        [SerializeField]
        private Vector3 _landlordEnterCameraPosition = default,
            _landlordEnterCameraRotation = default,
            _landlordSinkCameraPosition = default,
            _landlordSinkCameraRotation = default;
        [SerializeField]
        private Vector3 _tablePositionForCover, _tableRotationForCover;
        [SerializeField]
        private Transform _coffeeTableTransform;
        [SerializeField]
        private Waypoint _pcCoverWaypoint;
        [SerializeField]
        private DoorController _havenDoors;
        [SerializeField]
        private IInteractable _bathroomDoorsDialogue;
        [SerializeField]
        private DoorController _bathroomDoors;
        [Title("Action")]
        [SerializeField]
        private ICreature _dominic;
        [SerializeField]
        private ICreature _kieran;
        [SerializeField]
        private Waypoint _dominicWaypoint, _kieranWaypoint, _dominicFridgeWaypoint, _kieranFridgeWaypoint, _pcFridgeWaypoint, _dominicDoorWaypoint;
        [SerializeField]
        private Vector3 _cameraPositionDominicEnter, _cameraRotationDominicEnter, _cameraPositionPinnedToWall, _cameraRotationPinnedToWall;

        private CameraControlScript _cameraControl;

        protected override void Awake()
        {
            base.Awake();
            HavenDialogueCommands.HavenSceneLogic = this;
        }

        protected override void Start()
        {
            this.TryFindFirstGameObjectWithTag(TagConstants.CAMERA_RIG, out GameObject cameraControlGO);
            _cameraControl = cameraControlGO?.GetComponent<CameraControlScript>();
            base.Start();
        }

        public async override Task StartSceneAsync()
        {
            await base.StartSceneAsync();
            await new WaitForUpdate();
            GameManager.Instance.CurrentState = GameState.Exploration;
            await new WaitForSeconds(2f);
            PartyManager.Instance.MainCharacter.References.GameObject.SetActive(false);
            _cameraControl.ForceToPosition(_cameraStartPoint);
            _cameraControl.ForceRotation(_cameraStartRotation);
            DialogueManager.Instance.StartDialogue(_wakeUpStartNode, _sceneDialogues);
        }

        private async Task MovePCToPositionAfterDialogue(ICreature _desiree)
        {
            SceneLoadingUIManager fade = SceneLoadingUIManager.Instance;
            await fade.DoFadeOutAsync(.5f);
            await new WaitForUpdate();
            _desireeOnBed.SetActive(false);
            _desiree.References.GameObject.SetActive(true);
            await new WaitForSeconds(.5f);
            await fade.DoFadeInAsync(.5f);
        }



        public async void BringKevinForVisit()
        {
            SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
            await fade.DoFadeOutAsync(.5f);
            _landlord.ForceToPosition(_landlordSpawnWaypoint.transform.position);
            PartyManager.Instance.MainCharacter.ForceToPosition(_pcLandlordVisitWaypoint.transform.position);
            _cameraControl.ForceToPosition(_landlordEnterCameraPosition);
            _cameraControl.ForceRotation(_landlordEnterCameraRotation);
            await new WaitForFixedUpdate();
            //_ = PartyManager.MainCharacter.FaceTarget(_pcLandlordVisitWaypoint.FaceDirection);
            await new WaitForSeconds(.5f);
            DialogueManager.Instance.StartDialogue(_landlordVisitDialogueEntryNode, _sceneDialogues);
            await fade.DoFadeInAsync(.5f);
        }

        public async Task MoveToSink()
        {
            SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
            await fade.DoFadeOutAsync(.5f);
            _landlord.ForceToPosition(_landlordSinkWaypoint.transform.position);
            PartyManager.Instance.MainCharacter.ForceToPosition(_pcLandlordSinkWaypoint.transform.position);
            _cameraControl.ForceToPosition(_landlordSinkCameraPosition);
            _cameraControl.ForceRotation(_landlordSinkCameraRotation);
            await new WaitForFixedUpdate();
            //_ = PartyManager.MainCharacter.FaceTarget(_pcLandlordSinkWaypoint.FaceDirection);
            await new WaitForSeconds(.5f);
            await fade.DoFadeInAsync(.5f);
        }

        private void DisableSinkInteraction()
        {
            _kitchenSink.Interactable = false;
        }

        private void KillTheLandlord()
        {
            _landlord.References.StatsManager.Die();
        }

        public async void BargeIn()
        {
            SceneLoadingUIManager fade = SceneLoadingUIManager.Instance;
            await fade.DoFadeOutAsync(.5f);
            _coffeeTableTransform.position = _tablePositionForCover;
            _coffeeTableTransform.eulerAngles = _tableRotationForCover;
            _havenDoors.Open = true;
            _dominic.References.GameObject.SetActive(true);
            _kieran.References.GameObject.SetActive(true);
            _coffeeTableTransform.position = _tablePositionForCover;
            _coffeeTableTransform.eulerAngles = _tableRotationForCover;
            PartyManager.Instance.MainCharacter.ForceToPosition(_pcCoverWaypoint.transform.position);
            _dominic.ForceToPosition(_dominicWaypoint.transform.position);
            _kieran.ForceToPosition(_kieranWaypoint.transform.position);
            _cameraControl.ForceToPosition(_cameraPositionDominicEnter);
            await new WaitForFixedUpdate();
            _cameraControl.ForceRotation(_cameraRotationDominicEnter);
            //_ = PartyManager.Instance.MainCharacter.FaceTarget(_pcCoverWaypoint.FaceDirection);
            //_ = _dominic.FaceTarget(_dominicWaypoint.FaceDirection);
            //_ = _kieran.FaceTarget(_kieranWaypoint.FaceDirection);
            await new WaitForFixedUpdate();
            await fade.DoFadeInAsync(.5f);
        }

        private async void MoveActorsAndCameraToFridgeConfig()
        {
            SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
            await fade.DoFadeOutAsync(.5f);
            PartyManager.Instance.MainCharacter.ForceToPosition(_pcFridgeWaypoint.transform.position);
            _dominic.ForceToPosition(_dominicFridgeWaypoint.transform.position);
            _kieran.ForceToPosition(_kieranFridgeWaypoint.transform.position);
            _cameraControl.ForceToPosition(_cameraPositionPinnedToWall);
            await new WaitForFixedUpdate();
            _cameraControl.ForceRotation(_cameraRotationPinnedToWall);
            //_ = PartyManager.MainCharacter.FaceTarget(_pcFridgeWaypoint.FaceDirection);
            //_ = _dominic.FaceTarget(_dominicFridgeWaypoint.FaceDirection);
            //_ = _kieran.FaceTarget(_kieranFridgeWaypoint.FaceDirection);
            await fade.DoFadeInAsync(.5f);
        }

        private void DisableInteractionsBeforeDominic()
        {
            _gun.GetComponent<IInteractable>().Interactable = true;
            _crowbar.GetComponent<IInteractable>().Interactable = true;
        }

        public void RecruitKieran()
        {
            PartyManager.Instance.RecruitCharacter(_kieran);
        }

        public async void MoveDominicToDoorAndDestroy()
        {
            await _dominic.PerformAction(new Move(_dominic, _dominicDoorWaypoint.transform.position, .4f));
            Destroy(_dominic.References.GameObject);
        }



        //public async void QuitGame()
        //{
        //    SceneLoadingUIManager loading = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
        //    await loading.DoFadeOutAsync(.5f);
        //    //await LevelLoader.Instance.UnloadGameScene();
        //    this.FindFirstComponentWithTag<MainMenuUIManager>(TagConstants.MAIN_MENU_UI).gameObject.SetActive(true);
        //    this.FindFirstComponentWithTag<GameplayUIManager>(TagConstants.GAMEPLAY_UI).gameObject.SetActive(false);
        //    await loading.DoFadeInAsync(.5f);
        //}

        private class HavenSceneData : SceneLogicData
        {

        }

        public static class HavenDialogueCommands
        {
            public static HavenSceneLogic HavenSceneLogic;

            [YarnCommand("GetUpFromBedDesiree")]
            public async static void GetUpFromBedDesiree()
            {
                await HavenSceneLogic.MovePCToPositionAfterDialogue(PartyManager.Instance.MainCharacter);
            }

            [YarnCommand("OpenBathroomDoors")]
            public static void OpenBathroomDoors()
            {
                HavenSceneLogic._bathroomDoorsDialogue.Interactable = false;
                HavenSceneLogic._bathroomDoors.Open = true;
            }

            [YarnCommand("DestroyBathroomDoors")]
            public static void DestroyBathroomDoors()
            {
                HavenSceneLogic._bathroomDoors.gameObject.SetActive(false);
            }

            [YarnCommand("MoveLandlordAndPCToSinkPositions")]
            public async static void MoveLandlordAndPCToSinkPositions()
            {
                await HavenSceneLogic.MoveToSink();
            }

            [YarnCommand("KillKevin")]
            public static void KillKevin()
            {
                HavenSceneLogic.KillTheLandlord();
            }

            [YarnCommand("AddBobbyPinToInventory")]
            public static void AddBobbyPinToInventory()
            {
                Debug.LogError("Should add bobby pin to inventory");
            }

            [YarnCommand("DisableInteractionsBeforeDominic")]
            public static void DisableInteractionsBeforeDominic()
            {
                HavenSceneLogic.DisableInteractionsBeforeDominic();
            }

            [YarnCommand("HandleAltercationWithDominic")]
            public static void HandleAltercationWithDominic()
            {
                HavenSceneLogic.MoveActorsAndCameraToFridgeConfig();
            }
        }
    }
}
