using SunsetSystems.Entities.Characters;
using SunsetSystems.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class DevCurrentActiveActorChanger : MonoBehaviour
{
    #region Enable&Disable
    private void OnEnable()
    {
        CombatManager.ActiveActorChanged += OnActiveActorChanged;
    }
    private void OnDisable()
    {
        CombatManager.ActiveActorChanged -= OnActiveActorChanged;
    }
    #endregion

    private List<Creature> activeSceneCreatures;
    private Dropdown dropdown;
    private CombatManager combatManager;

    private void Start()
    {
        dropdown = GetComponent<Dropdown>();
        combatManager = CombatManager.Instance;
        activeSceneCreatures = combatManager.Actors;
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new();
        foreach (Creature c in activeSceneCreatures)
        {
            options.Add(new Dropdown.OptionData(c.gameObject.ToString()));
        }
        dropdown.AddOptions(options);
        if (CombatManager.CurrentActiveActor != null)
            dropdown.SetValueWithoutNotify(activeSceneCreatures.IndexOf(CombatManager.CurrentActiveActor));
    }

    private void OnActiveActorChanged(Creature newActor, Creature oldActor)
    {
        activeSceneCreatures = combatManager.Actors;
        dropdown.SetValueWithoutNotify(activeSceneCreatures.IndexOf(newActor));
    }
}
