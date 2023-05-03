using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace SunsetSystems
{
    public class Unit : MonoBehaviour
    {
        private const int ACTION_POINTS_MAX = 2;


        public static event EventHandler OnAnyActionPointsChanged;
        public static event EventHandler OnAnyUnitSpawned;
        //public static event EventHandler OnAnyUnitDead;


        [SerializeField] private bool isEnemy;


        private GridPosition gridPosition;
        //private HealthSystem healthSystem;
        private MoveAction moveAction;
        //private ReloadAction reloadAction;
        //private ShootAction shootAction;
        private BaseAction[] baseActionArray;
        private int actionPoints = ACTION_POINTS_MAX;

        private void Awake()
        {
            //healthSystem = GetComponent<HealthSystem>();
            moveAction = GetComponent<MoveAction>();
            //shootAction = GetComponent<ShootAction>();
            //reloadAction = GetComponent<ReloadAction>();
            baseActionArray = GetComponents<BaseAction>();
        }

        private void Start()
        {
            gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

            //healthSystem.OnDead += HealthSystem_OnDead;

            OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
        }

        private void Update()
        {
            GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            if (newGridPosition != gridPosition)
            {
                // Unit changed Grid Position
                GridPosition oldGridPosition = gridPosition;
                gridPosition = newGridPosition;

                LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
            }
        }

        public MoveAction GetMoveAction()
        {
           return moveAction;
        }

        //public ReloadAction GetReloadAction()
        //{
        //   return reloadAction;
        //}

        //  public ShootAction GetShootAction()
        //{
        //    return shootAction;
        //}

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }

        public Vector3 GetWorldPosition()
        {
            return transform.position;
        }

        public BaseAction[] GetBaseActionArray()
        {
            return baseActionArray;
        }

        public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
        {
            if (CanSpendActionPointsToTakeAction(baseAction))
            {
                SpendActionPoints(baseAction.GetActionPointsCost());
                return true;
            } else
            {
                return false;
            }
        }

        public bool CanSpendActionPointsToTakeAction(BaseAction baseAction)
        {
            if (actionPoints >= baseAction.GetActionPointsCost())
            {
                return true;
            } else
            {
                return false;
            }
        }

        private void SpendActionPoints(int amount)
        {
            actionPoints -= amount;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetActionPoints()
        {
           return actionPoints;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
                (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
            {
                actionPoints = ACTION_POINTS_MAX;

                OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsEnemy()
        {
           return isEnemy;
        }

        //public void Damage(int damageAmount)
        //{
        //   healthSystem.Damage(damageAmount);
        //}

        //private void HealthSystem_OnDead(object sender, EventArgs e)
        //{
        //    LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);

        //    Destroy(gameObject);

        //    OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
        //}

        //   public float GetHealthNormalized()
        //{
            //return healthSystem.GetHealthNormalized();
        //}

    }
}
