using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SunsetSystems.Game;
using SunsetSystems.Combat;
using Zenject;

[RequireComponent(typeof(TextMeshProUGUI))]
public class RoundCounter : MonoBehaviour
{
    private TextMeshProUGUI text;

    private ICombatManager combatManager;
    private IGameManager gameManager;

    [Inject]
    public void InjectDependencies(ICombatManager combatManager, IGameManager gameManager)
    {
        this.combatManager = combatManager;
        this.gameManager = gameManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.IsCurrentState(GameState.Combat))
            text.text = combatManager.GetRound().ToString();
    }
}
