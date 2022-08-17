using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SunsetSystems.Game;

[RequireComponent(typeof(TextMeshProUGUI))]
public class RoundCounter : MonoBehaviour
{
    private TextMeshProUGUI text;

    private TurnCombatManager turnCombatManager;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "";
        turnCombatManager = TurnCombatManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsCurrentState(GameState.Combat))
            text.text = turnCombatManager.GetRound().ToString();
    }
}
