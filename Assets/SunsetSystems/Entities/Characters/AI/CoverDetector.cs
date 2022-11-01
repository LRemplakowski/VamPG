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
        Vector3 sphereOrigin = new Vector3(gridPos.transform.position.x, gridPos.transform.position.y + instance.coverDetectionRadius + 0.1f, gridPos.transform.position.z);
        Collider[] colliders = Physics.OverlapSphere(sphereOrigin, instance.coverDetectionRadius, instance.coverLayerMask);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out Cover cover))
                coverSources.Add(cover);
        }
        return true;
    }
}
