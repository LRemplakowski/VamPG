using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridElement : MonoBehaviour
{
    public Vector2 GridPosition { get; set; }

    private bool _isMouseOver = false;
    public bool MouseOver {
        get => _isMouseOver;
        set
        {
            _isMouseOver = value;
            SetHoverHighlight(value);
        }
    }

    public Material hover, idle;
    private MeshRenderer meshRenderer;

    public void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void SetHoverHighlight(bool hovered)
    {
        if (hovered)
        {
            meshRenderer.material = hover;
        }
        else
        {
            meshRenderer.material = idle;
        }
    }

    public static bool IsInstance(GameObject gameObject)
    {
        return gameObject.GetComponent<GridElement>() != null;
    }
}
