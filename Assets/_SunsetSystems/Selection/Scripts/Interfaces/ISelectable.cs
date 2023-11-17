using SunsetSystems.Entities.Characters;
using System;
using UnityEngine;

namespace InsaneSystems.RTSSelection
{
    public interface ISelectable
    {
        event Action OnSelected, OnUnselected;
        
        void Select();
        void Unselect();

        Transform GetTransform();
        Collider GetCollider();

        Creature GetCreature();
    }
}