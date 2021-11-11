using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

public class CoverDetector : Singleton<CoverDetector>
{
    [SerializeField]
    private float coverDetectionRadius = 1f;
    [SerializeField]
    private LayerMask coverLayerMask;

    public static bool IsPositionNearCover(GridElement gridPos, out List<Cover> coverSources)
    {
        coverSources = new List<Cover>();
        Vector3 sphereOrigin = new Vector3(gridPos.transform.position.x, gridPos.transform.position.y + Instance.coverDetectionRadius + 0.1f, gridPos.transform.position.z);
        Collider[] colliders = Physics.OverlapSphere(sphereOrigin, Instance.coverDetectionRadius, Instance.coverLayerMask);
        foreach (Collider col in colliders)
        {
            Cover cover = col.GetComponent<Cover>();
            if (cover)
                coverSources.Add(cover);
        }
        return true;
    }
}
