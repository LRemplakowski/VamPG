using System;
using System.Collections;
using UnityEngine;

public class CombatAI : ExposableMonobehaviour
{
    [SerializeField, ReadOnly]
    private CombatBehaviour actorBehaviour;

    #region Enable&Disable
    private void OnEnable()
    {
        TurnCombatManager.instance.onCombatStart += OnCombatStart;
        TurnCombatManager.instance.onCombatEnd += OnCombatEnd;
        TurnCombatManager.instance.onCombatRoundBegin += OnCombatRoundBegin;
        TurnCombatManager.instance.onCombatRoundEnd += OnCombatRoundEnd;
    }

    private void OnDisable()
    {
        TurnCombatManager.instance.onCombatStart -= OnCombatStart;
        TurnCombatManager.instance.onCombatEnd -= OnCombatEnd;
        TurnCombatManager.instance.onCombatRoundBegin -= OnCombatRoundBegin;
        TurnCombatManager.instance.onCombatRoundEnd -= OnCombatRoundEnd;
    }
    #endregion

    private void OnCombatRoundEnd(Creature currentActor)
    {
        
    }

    private void OnCombatRoundBegin(Creature currentActor)
    {
        actorBehaviour = currentActor.GetComponent<CombatBehaviour>();
        actorBehaviour.HasActed = false;
        actorBehaviour.HasMoved = false;
    }

    private void OnCombatEnd()
    {
        StopCoroutine(ProcessCombatActions());
    }

    private void OnCombatStart()
    {
        ForceCreaturesIntoNearestSquares();
        StartCoroutine(ProcessCombatActions());
    }

    private IEnumerator ProcessCombatActions()
    {
        Debug.Log("foo");
        yield return new WaitUntil(() => actorBehaviour != null);
        Debug.Log("bar");
        while (actorBehaviour != null)
        {
            Debug.Log("ProcessCombatActions");
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void ForceCreaturesIntoNearestSquares()
    {
        TurnCombatManager.instance.GetCreaturesInCombat().ForEach(c => c.Move(FindObjectOfType<GridController>().GetNearestGridElement(c.transform.position)));
    }
}