using SunsetSystems.Entities.Characters;
using SunsetSystems.Entities.Cover;
using SunsetSystems.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverDetector : Singleton<CoverDetector>
{
    [SerializeField]
    private float coverDetectionRadius = 1f;
    [SerializeField]
    private LayerMask coverLayerMask;

    public static bool IsPositionNearCover(GridElement gridPos, out List<Cover> coverSources)
    {
        coverSources = new List<Cover>();
        Collider[] colliders = Physics.OverlapSphere(gridPos.transform.position, instance.coverDetectionRadius * gridPos.transform.localScale.x, instance.coverLayerMask);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out Cover cover))
                coverSources.Add(cover);
        }
        return true;
    }

    public static bool FiringLineObstructedByCover(Creature attacker, Creature target, out Cover coverSource)
    {
        if (IsPositionNearCover(target.CurrentGridPosition, out List<Cover> coverSources))
        {
            Vector3 attackerRaycast = attacker.CombatBehaviour.RaycastOrigin;
            Vector3 targetRaycast = target.CombatBehaviour.RaycastOrigin;
            float distance = Vector3.Distance(attackerRaycast, targetRaycast);
            Vector3 direction = (attackerRaycast - targetRaycast).normalized;
            RaycastHit[] hits = Physics.RaycastAll(new Ray(attackerRaycast, direction), distance, instance.coverLayerMask);
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    coverSource = hit.collider.GetComponent<Cover>();
                    if (coverSources.Contains(coverSource))
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
