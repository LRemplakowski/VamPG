using SunsetSystems.Entities.Characters;
using System;
using UnityEngine;

namespace InsaneSystems.RTSSelection
{
    public class Selectable : MonoBehaviour, ISelectable
    {
        public event Action OnSelected, OnUnselected;
        private Collider Collider { get => GetComponent<Collider>(); }

        void OnDestroy() => Selection.AllSelectables.Remove(this);
        
        public void Select() => OnSelected?.Invoke();
        public void Unselect() => OnUnselected?.Invoke();

        public Transform GetTransform() => transform;
        public Collider GetCollider() => Collider;
        public Creature GetCreature() => GetComponent<Creature>();
    }
}