using System;
using System.Collections.Generic;
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

    private void Start()
    {
        dropdown = GetComponent<Dropdown>();
        activeSceneCreatures = TurnCombatManager.Instance.GetCreaturesInCombat();
        dropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (Creature c in activeSceneCreatures)
        {
            options.Add(new Dropdown.OptionData(c.gameObject.ToString()));
        }
        dropdown.AddOptions(options);
        if (TurnCombatManager.Instance.CurrentActiveActor != null)
            dropdown.SetValueWithoutNotify(activeSceneCreatures.IndexOf(TurnCombatManager.Instance.CurrentActiveActor));
    }

    private void OnActiveActorChanged(Creature newActor, Creature oldActor)
    {
        activeSceneCreatures = TurnCombatManager.Instance.GetCreaturesInCombat();
        dropdown.SetValueWithoutNotify(activeSceneCreatures.IndexOf(newActor));
    }
}
