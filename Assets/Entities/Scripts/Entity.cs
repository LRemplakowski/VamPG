using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Entity : ExposableMonobehaviour
{
    public static bool IsInteractable(GameObject entity)
    {
        return entity.GetComponent<IInteractable>() != null;
    }
}
