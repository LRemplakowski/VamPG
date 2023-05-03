using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace SunsetSystems
{
    public class UnitSelectedVisual : MonoBehaviour
    {

        [SerializeField] private Unit unit;

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            UnitSystem.Instance.OnSelectedUnitChanged += UnitSystem_OnSelectedUnitChanged;

            UpdateVisual();
        }

        private void UnitSystem_OnSelectedUnitChanged(object sender, EventArgs empty)
        {
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (UnitSystem.Instance.GetSelectedUnit() == unit)
            {
                meshRenderer.enabled = true;
            }
            else
            {
                meshRenderer.enabled = false;
            }
        }

        private void OnDestroy()
        {
            UnitSystem.Instance.OnSelectedUnitChanged -= UnitSystem_OnSelectedUnitChanged;
        }

    }
}
