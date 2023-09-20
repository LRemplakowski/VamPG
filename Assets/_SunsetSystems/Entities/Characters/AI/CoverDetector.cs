using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities;
using SunsetSystems.Entities.Interfaces;
using SunsetSystems.Utils;
using System.Collections.Generic;
using UnityEngine;
using SunsetSystems.Combat.Grid;
using SunsetSystems.Combat;

public class CoverDetector : Singleton<CoverDetector>
{
    [SerializeField]
    private float coverDetectionRadius = 1f;
    [SerializeField]
    private LayerMask coverLayerMask;

    public static bool IsPositionNearCover(IGridCell gridPos, out List<ICover> coverSources)
    {
        coverSources = new List<ICover>();
        Collider[] colliders = Physics.OverlapSphere(gridPos.WorldPosition, instance.coverDetectionRadius * gridPos.CellSize, instance.coverLayerMask);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out ICover cover))
                coverSources.Add(cover);
        }
        return true;
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
