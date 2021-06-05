using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class GridElement : ExposableMonobehaviour
{
    [SerializeField]
    public Vector2Int GridPosition { get; set; }
    [SerializeField]
    public Status Visited { get; set; }

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
    private Transform visualTransform;

    public void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        visualTransform = GetComponentInChildren<Transform>();
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

    public override string ToString()
    {
        return "GridElement(X: "+GridPosition.x+", Y: "+GridPosition.y+", isActive:"+gameObject.activeSelf+", visited: "+Visited+")";
    }

    public enum Status
    {
        NotVisited = -1,
        Visited = 1,
        Occupied = 0,
        ProvidesCover = -2
    }
}
