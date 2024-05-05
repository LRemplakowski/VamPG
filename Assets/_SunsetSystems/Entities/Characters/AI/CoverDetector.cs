using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Utils;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Combat;

public class CoverDetector : MonoBehaviour
{
    [SerializeField]
    private float coverDetectionRadius = 1f;
    [SerializeField]
    private LayerMask coverLayerMask;

    private static CoverDetector instance;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public static bool FiringLineObstructedByCover(ICombatant attacker, ICombatant target, out ICover coverSource)
    {
        if (target.IsInCover)
        {
            Vector3 attackerRaycast = attacker.AimingOrigin;
            Vector3 targetRaycast = target.AimingOrigin;
            float distance = Vector3.Distance(attackerRaycast, targetRaycast);
            Vector3 direction = (attackerRaycast - targetRaycast).normalized;
            RaycastHit[] hits = Physics.RaycastAll(new Ray(attackerRaycast, direction), distance, instance.coverLayerMask);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    coverSource = hit.collider.GetComponent<ICover>();
                    if (target.CurrentCoverSources.Contains(coverSource))
                    {
                        return true;
                    }
                }
            }
        }
        coverSource = null;
        return false;
    }
}
