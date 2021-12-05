using Entities.Characters;
using System;
using System.Collections.Generic;
using SunsetSystems.Management;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class DevCurrentActiveActorChanger : ExposableMonobehaviour
{
    #region Enable&Disable
    private void OnEnable()
    {
        TurnCombatManager.onActiveActorChanged += OnActiveActorChanged;
    }
    private void OnDisable()
    {
        TurnCombatManager.onActiveActorChanged -= OnActiveActorChanged;
    }
    #endregion

    private List<Creature> activeSceneCreatures;
    private Dropdown dropdown;
    private TurnCombatManager turnCombatManager;

    private void Start()
    {
        dropdown = GetComponent<Dropdown>();
        turnCombatManager = ReferenceManager.GetManager<TurnCombatManager>();
        activeSceneCreatures = turnCombatManager.GetCreaturesInCombat();
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (Creature c in activeSceneCreatures)
        {
            options.Add(new Dropdown.OptionData(c.gameObject.ToString()));
        }
        dropdown.AddOptions(options);
        if (turnCombatManager.CurrentActiveActor != null)
            dropdown.SetValueWithoutNotify(activeSceneCreatures.IndexOf(turnCombatManager.CurrentActiveActor));
    }

    private void OnActiveActorChanged(Creature newActor, Creature oldActor)
    {
        activeSceneCreatures = turnCombatManager.GetCreaturesInCombat();
        dropdown.SetValueWithoutNotify(activeSceneCreatures.IndexOf(newActor));
    }
}
