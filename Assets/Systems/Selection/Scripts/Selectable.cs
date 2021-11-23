using System;
using UnityEngine;

namespace InsaneSystems.RTSSelection
{
    public class Selectable : MonoBehaviour, ISelectable
    {
        public event Action OnSelected, OnUnselected;
        [SerializeField, ReadOnly]
        private new Collider collider;

        private void Awake()
        {
            collider = GetComponent<Collider>();
        }

        void OnDestroy() => Selection.AllSelectables.Remove(this);
        
        public void Select() => OnSelected?.Invoke();
        public void Unselect() => OnUnselected?.Invoke();

        public Transform GetTransform() => transform;
        public Collider GetCollider() => collider;
        public Creature GetCreature() => GetComponent<Creature>();
    }
}