using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ICover
    {
        CoverQuality Quality { get; }

        Vector3 WorldPosition { get; }
    }

    public enum CoverQuality
    {
        None, Half, Full
    }
}
