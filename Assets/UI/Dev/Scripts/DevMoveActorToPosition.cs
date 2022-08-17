using Entities.Characters;
using Entities.Characters.Actions;
using SunsetSystems.Combat;
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
        CombatManager.Instance.CurrentEncounter.MyGrid.Dev_SetWholeGridActive();
    }

    private void OnMovementStarted(Creature who)
    {
        InputOverride = false;
    }
}
