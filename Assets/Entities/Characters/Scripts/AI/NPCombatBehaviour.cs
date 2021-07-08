using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public class NPCombatBehaviour : CombatBehaviour
{
    List<GridElement> elementsInRange = new List<GridElement>();

    protected override bool EvaluateEndTurnCondition()
    {
        return HasMoved && HasActed;
    }

    protected override void OnCombatRoundBeginImpl(Creature newActor)
    {
        Debug.Log(owner.gameObject.name + " begins combat round!");
        HasActed = false;
        HasMoved = false;

        AIAction action = DetermineActionToTake();
        elementsInRange = TurnCombatManager.instance.GridInstance.GetElementsInRangeOfActor(owner);
        PerformAction(action);
        if (HasMoved)
        {
            PerformAction(AIAction.Attack);
            HasActed = true;
        }
        else if (HasActed)
        {
            PerformAction(AIAction.MoveToCover);
        }
        else
        {
            Debug.Log("No possible moves?");
            HasActed = true;
            HasMoved = true;
        }
    }

    protected override void OnCombatRoundEndImpl(Creature currentActor)
    {
        Debug.Log(owner.gameObject.name + " ends combat round!");
    }

    protected override void OnCombatStartImpl()
    {
        GridController gridController = GameManager.GetGridController();
        GridElement g = gridController.GetNearestGridElement(this.transform.position);
        owner.Move(g);
    }

    protected override void OnLookForCoverFinishedImpl(Dictionary<GridElement, List<Cover>> positionsNearCover)
    {
        Debug.LogWarning("Positions near cover: " + positionsNearCover.Count);
        if (positionsNearCover.Count <= 0)
        {
            MoveToRandomSquare();
        }
        else
        {
            MoveToRandomPossibleCover(positionsNearCover);
        }
    }

    protected override void OnMovementFinishedImpl(Creature who)
    {
        HasMoved = true;
    }

    protected override void OnMovementStartedImpl(Creature who)
    {

    }

    public AIAction DetermineActionToTake()
    {
        return (AIAction)UnityEngine.Random.Range(0, Enum.GetValues(typeof(AIAction)).Length);
    }

    public bool PerformAction(AIAction action)
    {
        switch (action)
        {
            case AIAction.Attack:
                HasActed = PerformAttack(out Creature target);
                Debug.Log(owner.gameObject.name + " attacked " + target.gameObject.name);
                return HasActed;
            case AIAction.MoveRandom:
                return PerformRandomMove();
            case AIAction.MoveToCover:
                if (CanMove())
                {
                    coverDetector.StartLookingForCover(elementsInRange);
                    return true;
                }
                return false;
            case AIAction.UseItem:
                Debug.Log(owner.gameObject.name + " uses an item!");
                HasActed = true;
                break;
            case AIAction.UsePower:
                Debug.Log(owner.gameObject.name + " uses a power!");
                HasActed = true;
                break;
            default:
                break;
        }
        return false;
    }

    private bool PerformAttack(out Creature target)
    {
        List<Creature> targets = TurnCombatManager.instance.GetCreaturesInCombat().FindAll(c => c.IsOfType(typeof(Player)));
        //Let's filter targets that are not in range of attack
        float range = GetComponent<StatsManager>().GetAttackRange();
        if (range > 0)
        {
            targets = targets.FindAll(c => Vector3.Distance(c.transform.position, this.transform.position) <= range);
        }
        else
        {
            List<GridElement> adjacentElements = TurnCombatManager.instance.GridInstance.GetAdjacentGridElements(owner.CurrentGridPosition);
            targets = targets.FindAll(c => adjacentElements.Contains(c.CurrentGridPosition));
        }

        if (targets.Count > 0)
        {
            target = DetermineBestTarget(targets);
            owner.Attack(target);
            return true;
        }
        else
        {
            target = owner;
            return false;
        }
    }

    private Creature DetermineBestTarget(List<Creature> targets)
    {
        Creature best = targets[0];
        foreach (Creature c in targets)
        {
            if (c.GetComponent<StatsManager>().GetDefensePool() < best.GetComponent<StatsManager>().GetDefensePool())
                best = c;
        }
        return best;
    }

    private bool PerformRandomMove()
    {
        if (CanMove())
        {
            MoveToRandomSquare();
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CanMove()
    {
        return true;
    }

    private void MoveToRandomSquare()
    {
        Debug.Log(owner.gameObject.name + " moving to random square!");
        int index = UnityEngine.Random.Range(0, elementsInRange.Count);
        owner.Move(elementsInRange[index]);
    }

    private void MoveToRandomPossibleCover(Dictionary<GridElement, List<Cover>> positionsNearCover)
    {
        Debug.Log(owner.gameObject.name + " moving to random possible cover!");
        int random = UnityEngine.Random.Range(0, positionsNearCover.Count);
        int index = 0;
        foreach (KeyValuePair<GridElement, List<Cover>> pair in positionsNearCover)
        {
            if (index == random)
            {
                owner.Move(pair.Key);
                break;
            }
            index++;
        }
    }
}
