using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems.Combat
{
    public interface ICover
    {
        NewCoverQuality Quality { get; }

        Vector3 WorldPosition { get; }
    }

    public enum NewCoverQuality
    {
        None, Half, Full
    }
}
