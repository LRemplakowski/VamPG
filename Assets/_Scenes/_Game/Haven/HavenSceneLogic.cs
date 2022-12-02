using Redcode.Awaiting;
using SunsetSystems.Data;
using SunsetSystems.Dialogue;
using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Interactable;
using SunsetSystems.Input.CameraControl;
using SunsetSystems.Inventory;
using SunsetSystems.Inventory.Data;
using SunsetSystems.Party;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Yarn.Unity;

namespace SunsetSystems.Loading
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
        [Header("Prologue")]
        [SerializeField]
        private GameObject _desireeOnBed;
        [SerializeField]
        private GameObject _handgun;
        [SerializeField]
        private GameObject _crowbar, _gun;
        [SerializeField]
        private Weapon _handgunItem;
        [SerializeField]
        private List<InteractableEntity> _interactablesToEnableAfterPhoneCall = new();
        [SerializeField]
        private DialogueEntity _kitchenSink;
        [SerializeField]
        private string landlordDialogueEntryNode;
        [SerializeField]
        private DialogueEntity _phone;
        [SerializeField]
        private TalkableNPC _landlord;
        [SerializeField]
        private Waypoint _landlordSpawnWaypoint, _landlordSinkWaypoint, _pcLandlordVisitWaypoint, _pcLandlordSinkWaypoint;
        [SerializeField]
        private DefaultInteractable _apartmentDoorLandlordInteraction;
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
        private Doors _havenDoors;
        [SerializeField]
        private DialogueEntity _bathroomDoorsDialogue;
        [SerializeField]
        private Doors _bathroomDoors;
        [Header("Action")]
        [SerializeField]
        private Creature _dominic;
        [SerializeField]
        private Creature _kieran;
        [SerializeField]
        private Waypoint _dominicWaypoint, _kieranWaypoint;
        [SerializeField]
        private Vector3 _cameraPositionDominicEnter, _cameraRotationDominicEnter, _cameraPositionPinnedToWall, _cameraRotationPinnedToWall;

        private CameraControlScript _cameraControl;

        protected override void Awake()
        {
            base.Awake();
            HavenDialogueCommands.HavenSceneLogic = this;
        }

        private void Start()
        {
            this.TryFindFirstGameObjectWithTag(TagConstants.CAMERA_RIG, out GameObject cameraControlGO);
            _cameraControl = cameraControlGO?.GetComponent<CameraControlScript>();
        }

        public async override Task StartSceneAsync(SceneLoadingData data)
        {
            await base.StartSceneAsync(data);
            await new WaitForUpdate();
            PartyManager.MainCharacter.Agent.Warp(new Vector3(100, 100, 100));
            await new WaitForSeconds(2);
            DialogueManager.Instance.StartDialogue(_wakeUpStartNode, _sceneDialogues);
            _ = Task.Run(async () =>
            {
                await new WaitForUpdate();
                _cameraControl.ForceToPosition(_cameraStartPoint);
                _cameraControl.ForceRotation(_cameraStartRotation);
            });
        }

        private async Task MovePCToPositionAfterDialogue()
        {
            SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
            await fade.DoFadeOutAsync(.5f);
            await new WaitForUpdate();
            _desireeOnBed.SetActive(false);
            PartyManager.MainCharacter.Agent.Warp(_startPosition.position);
            await new WaitForSeconds(.5f);
            await new WaitForUpdate();
            await fade.DoFadeInAsync(.5f);
        }

        public async void BringKevinForVisit()
        {
            SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
            await fade.DoFadeOutAsync(.5f);
            _landlord.Agent.Warp(_landlordSpawnWaypoint.transform.position);
            PartyManager.MainCharacter.Agent.Warp(_pcLandlordVisitWaypoint.transform.position);
            _cameraControl.ForceToPosition(_landlordEnterCameraPosition);
            _cameraControl.ForceRotation(_landlordEnterCameraRotation);
            await new WaitForFixedUpdate();
            _ = _landlord.FaceTarget(_landlordSpawnWaypoint.FaceDirection);
            _ = PartyManager.MainCharacter.FaceTarget(_pcLandlordVisitWaypoint.FaceDirection);
            await new WaitForSeconds(.5f);
            DialogueManager.Instance.StartDialogue(_landlordVisitDialogueEntryNode, _sceneDialogues);
            await fade.DoFadeInAsync(.5f);
        }

        public async void MoveCoffeTableAndTakeCover()
        {
            SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
            await fade.DoFadeOutAsync(.5f);
            _coffeeTableTransform.position = _tablePositionForCover;
            _coffeeTableTransform.eulerAngles = _tableRotationForCover;
            PartyManager.MainCharacter.ForceCreatureToPosition(_pcCoverWaypoint.transform.position);
            await PartyManager.MainCharacter.FaceTarget(_pcCoverWaypoint.FaceDirection);
        }

        public async Task MoveToSink()
        {
            SceneLoadingUIManager fade = this.FindFirstComponentWithTag<SceneLoadingUIManager>(TagConstants.SCENE_LOADING_UI);
            await fade.DoFadeOutAsync(.5f);
            _landlord.Agent.Warp(_landlordSinkWaypoint.transform.position);
            PartyManager.MainCharacter.Agent.Warp(_pcLandlordSinkWaypoint.transform.position);
            _cameraControl.ForceToPosition(_landlordSinkCameraPosition);
            _cameraControl.ForceRotation(_landlordSinkCameraRotation);
            await new WaitForFixedUpdate();
            _ = _landlord.FaceTarget(_landlordSinkWaypoint.FaceDirection);
            _ = PartyManager.MainCharacter.FaceTarget(_pcLandlordSinkWaypoint.FaceDirection);
            await new WaitForSeconds(.5f);
            await fade.DoFadeInAsync(.5f);
        }

        private void DisableSinkInteraction()
        {
            _kitchenSink.Interactable = false;
        }

        private void KillTheLandlord()
        {
            _landlord.StatsManager.Die();
        }

        public async void BargeIn()
        {
            await new WaitForUpdate();
            _havenDoors.Interactable = true;
            _havenDoors.Interact();
            _havenDoors.Interactable = false;
            _dominic.gameObject.SetActive(true);
            _kieran.gameObject.SetActive(true);
            _dominic.MoveAndRotate(_dominicWaypoint.transform.position, _dominicWaypoint.FaceDirection);
            _kieran.MoveAndRotate(_kieranWaypoint.transform.position, _kieranWaypoint.FaceDirection);
            _coffeeTableTransform.position = _tablePositionForCover;
            _coffeeTableTransform.eulerAngles = _tableRotationForCover;
            PartyManager.MainCharacter.ForceCreatureToPosition(_pcCoverWaypoint.transform.position);
            await new WaitForFixedUpdate();
            PartyManager.MainCharacter.MoveAndRotate(_pcCoverWaypoint.transform.position, _dominic.transform);
        }

        private void DisableInteractionsBeforeDominic()
        {
            _interactablesToEnableAfterPhoneCall.ForEach(i => i.Interactable = false);
            _gun.GetComponent<IInteractable>().Interactable = true;
            _crowbar.GetComponent<IInteractable>().Interactable = true;
        }

        public void RecruitKieran()
        {
            PartyManager.RecruitCharacter(_kieran.Data);
        }

        private static class HavenDialogueCommands
        {
            public static HavenSceneLogic HavenSceneLogic;

            [YarnCommand("GetUpFromBedDesiree")]
            public async static void GetUpFromBedDesiree()
            {
                await HavenSceneLogic.MovePCToPositionAfterDialogue();
            }

            [YarnCommand("EnableInteractionsAfterPhoneCall")]
            public static void EnableInteractionsAfterPhoneCall()
            {
                HavenSceneLogic._bathroomDoorsDialogue.Interactable = true;
                HavenSceneLogic._interactablesToEnableAfterPhoneCall.ForEach(interactable => interactable.Interactable = true);
            }

            [YarnCommand("OpenBathroomDoors")]
            public static void OpenBathroomDoors()
            {
                HavenSceneLogic._bathroomDoorsDialogue.Interactable = false;
                HavenSceneLogic._bathroomDoors.Interactable = true;
                HavenSceneLogic._bathroomDoors.Interact();
                HavenSceneLogic._bathroomDoors.Interactable = false;
            }

            [YarnCommand("DestroyBathroomDoors")]
            public static void DestroyBathroomDoors()
            {
                HavenSceneLogic._bathroomDoors.Interactable = true;
                HavenSceneLogic._bathroomDoors.Interact();
                Destroy(HavenSceneLogic._bathroomDoors.gameObject);
            }

            [YarnCommand("ActivateApartmentDoorInteraction")]
            public static void ActivateApartmentDoorInteraction()
            {
                HavenSceneLogic._havenDoors.Interactable = true;
            }

            [YarnCommand("HandleGunTaken")]
            public static void HandleGunTaken()
            {
                HavenSceneLogic._handgun.gameObject.SetActive(false);
                InventoryManager.PlayerInventory.AddItem(new(HavenSceneLogic._handgunItem));
            }

            [YarnCommand("HandleCrowbarTaken")]
            public static void HandleCrowbarTaken()
            {
                HavenSceneLogic._crowbar.SetActive(false);
            }

            [YarnCommand("SetPhoneDialogueToLandlordDialogue")]
            public static void SetPhoneDialogueToLandlordDialogue()
            {
                HavenSceneLogic._phone.EntryNode = HavenSceneLogic.landlordDialogueEntryNode;
                HavenSceneLogic._phone.ResetInteraction();
            }

            [YarnCommand("EnableApartmentDoorLandlordDialogue")]
            public static void EnableApartmentDoorLandlordDialogue()
            {
                HavenSceneLogic._apartmentDoorLandlordInteraction.Interactable = true;
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
                Debug.LogException(new NotImplementedException());
            }

            [YarnCommand("DisableSinkInteraction")]
            public static void DisableSinkInteraction()
            {
                HavenSceneLogic.DisableSinkInteraction();
            }

            [YarnCommand("DisableInteractionsBeforeDominic")]
            public static void DisableInteractionsBeforeDominic()
            {
                HavenSceneLogic.DisableInteractionsBeforeDominic();
            }
        }
    }
}
