using Entities.Characters;
using Entities.Characters.Actions;
using System.Collections;
using System.Collections.Generic;
using SunsetSystems.Management;
using UnityEngine;

public class DevMoveActorToPosition : MonoBehaviour
{
    public static bool InputOverride { get; private set; }

    #region Enable&Disable
    private void OnEnable()
    {
        Move.onMovementStarted += OnMovementStarted;
    }

    private void OnDisable()
    {
        Move.onMovementStarted -= OnMovementStarted;
    }
    #endregion

    private void Start()
    {
        InputOverride = false;
    }

    public void OnButtonClick()
    {
        InputOverride = true;
        ReferenceManager.GetManager<TurnCombatManager>().GridInstance.Dev_SetWholeGridActive();
    }

    private void OnMovementStarted(Creature who)
    {
        InputOverride = false;
    }
}
