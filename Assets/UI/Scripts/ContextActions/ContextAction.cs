using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ContextAction : MonoBehaviour
{
    public static InventoryItem item;

    public abstract void OnClick();
}

