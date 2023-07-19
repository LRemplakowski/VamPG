using SunsetSystems.Entities.Characters.Actions;
using UnityEngine;

namespace SunsetSystems.Entities.Characters
{
    public abstract class AbstractNPC : Creature, INameplateReciever
    {
        [SerializeField]
        private Transform _lineTarget;

        public virtual string NameplateText { get => ShowNameplate ? Data.FullName : string.Empty; }
        public Vector3 NameplateWorldPosition => new(transform.position.x, transform.position.y + _nameplateOffset, transform.position.z);
        [SerializeField]
        protected float _nameplateOffset = 2.5f;
        [field: SerializeField]
        public bool ShowNameplate { get; set; } = true;

        public Transform LineTarget
        {
            get => _lineTarget;
            private set => _lineTarget = value;
        }

        public override Move Move(Vector3 moveTarget, float stoppingDistance)
        {
            Move moveAction = new(this, moveTarget, stoppingDistance);
            PerformAction(moveAction);
            return moveAction;
        }

        public override Move Move(Vector3 moveTarget)
        {
            return Move(moveTarget, 0f);
        }

        public override Move Move(GridElement moveTarget)
        {
            CurrentGridPosition = moveTarget;
            return Move(moveTarget.transform.position);
        }

        public override Move MoveAndRotate(Vector3 moveTarget, Transform rotationTarget)
        {
            ClearAllActions();
            Move moveAction = new(this, moveTarget, rotationTarget);
            PerformAction(moveAction);
            return moveAction;
        }

        public override Attack Attack(Creature target)
        {
            Attack attackAction = new(target, this);
            PerformAction(attackAction);
            return attackAction;
        }

        protected override void Start()
        {
            base.Start();
            if (!LineTarget)
                LineTarget = CreateDefaultLineTarget();
        }

        private Transform CreateDefaultLineTarget()
        {
            GameObject lt = new("Default Line Target");
            lt.transform.parent = this.transform;
            lt.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z);
            return lt.transform;
        }
    }
}
