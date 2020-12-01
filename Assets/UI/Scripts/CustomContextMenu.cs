using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GridLayoutGroup))]
public abstract class CustomContextMenu : MonoBehaviour
{
    public Vector3 offset = new Vector3(0,0,0);

    [SerializeField]
    private List<ContextAction> actions;
    [HideInInspector]
    private static CustomContextMenu instance;

    public void InvokeMenu(Vector3 position)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
            return;
        ClearContextMenu();

        instance = Instantiate(this, position+offset, Quaternion.identity, canvas.transform);
        foreach(ContextAction action in actions)
        {
            Instantiate(action).transform.SetParent(instance.transform, false);
        }
    }

    public static void ClearContextMenu()
    {
        if (instance != null)
            Destroy(instance.gameObject);
    }
}
