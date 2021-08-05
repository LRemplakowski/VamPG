using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverDetector : MonoBehaviour
{
    [SerializeField]
    private float coverDetectionRadius = 1f;
    [SerializeField]
    private LayerMask coverLayerMask;

    private static CoverDetector instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    public static bool IsPositionNearCover(GridElement gridPos, out List<Cover> coverSources)
    {
        coverSources = new List<Cover>();
        Vector3 sphereOrigin = new Vector3(gridPos.transform.position.x, gridPos.transform.position.y + instance.coverDetectionRadius + 0.1f, gridPos.transform.position.z);
        Collider[] colliders = Physics.OverlapSphere(sphereOrigin, instance.coverDetectionRadius, instance.coverLayerMask);
        foreach (Collider col in colliders)
        {
            Cover cover = col.GetComponent<Cover>();
            if (cover)
                coverSources.Add(cover);
        }
        return true;
    }

    public Dictionary<GridElement, List<Cover>> LookForCover(List<GridElement> gridElements)
    {
        Dictionary<GridElement, List<Cover>> result = new Dictionary<GridElement, List<Cover>>();
        foreach (GridElement g in gridElements)
        {
            if (IsPositionNearCover(g, out List<Cover> coverSources))
            {
                result.Add(g, coverSources);
            }
        }
        return result;
    }
}
