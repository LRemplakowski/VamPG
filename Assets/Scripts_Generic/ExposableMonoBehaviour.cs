using System;
using UnityEngine;

[Serializable]
public class ExposableMonobehaviour : MonoBehaviour 
{
    public bool IsOfType(System.Type t)
    {
        return this.GetType().IsAssignableFrom(t);
    }

}
