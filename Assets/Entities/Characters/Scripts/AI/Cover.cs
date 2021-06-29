using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : Entity
{
    [SerializeField]
    private CoverQuality coverQuality;

    public CoverQuality GetCoverQuality()
    {
        return coverQuality;
    }
}
