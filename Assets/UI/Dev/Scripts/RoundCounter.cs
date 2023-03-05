using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SunsetSystems.Game;
using SunsetSystems.Combat;

[RequireComponent(typeof(TextMeshProUGUI))]
public class RoundCounter : MonoBehaviour
{
    private TextMeshProUGUI text;

    private CombatManager turnCombatManager;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "";
        turnCombatManager = CombatManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.IsCurrentState(GameState.Combat))
            text.text = turnCombatManager.GetRound().ToString();
    }
}
