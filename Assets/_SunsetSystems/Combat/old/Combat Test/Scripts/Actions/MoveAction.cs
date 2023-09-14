using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SunsetSystems.Entities.Characters;

namespace SunsetSystems.Combat
{
    public class MoveAction : BaseAction
    {

        public event EventHandler OnStartMoving;
        public event EventHandler OnStopMoving;

        [SerializeField] private StatsManager statsManager;
        private int maxMoveDistance;

        private Vector3 targetPosition;

        protected override void Awake()
        {
            base.Awake();
            targetPosition = transform.position;
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            float stoppingDistance = .1f;
            if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
            {
                float moveSpeed = 4f;
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
            }
            else
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);

                ActionComplete();
            }

            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }


        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

            OnStartMoving?.Invoke(this, EventArgs.Empty);

            ActionStart(onActionComplete);
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();

            GridPosition unitGridPosition = unit.GetGridPosition();

            maxMoveDistance = statsManager.GetCombatSpeed();

            throw new NotImplementedException();
            //for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
            //{
            //    for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            //    {
            //        GridPosition offsetGridPosition = new GridPosition(x, z);
            //        GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

            //        if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
            //        {
            //            continue;
            //        }

            //        if (unitGridPosition == testGridPosition)
            //        {
            //            // Same Grid Position where the unit is already at
            //            continue;
            //        }

            //        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            //        {
            //            // Grid Position already occupied with another Unit
            //            continue;
            //        }

            //        validGridPositionList.Add(testGridPosition);
            //    }
            //}

            return validGridPositionList;
        }


        public override string GetActionName()
        {
            return "Move";
        }
        
    }

}
