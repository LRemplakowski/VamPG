using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class GridElement : MonoBehaviour
{
    public Vector3 WorldPosition { get; private set; }
    public Vector2Int GridPosition { get; set; }
    [SerializeField]
    private Status _visited;
    public Status Visited
    {
        get => _visited;
        set
        {
            _visited = value;
        }
    }

    private bool _isMouseOver = false;
    public bool MouseOver
    {
        get => _isMouseOver;
        set
        {
            _isMouseOver = value;
            SetHoverHighlight(value);
        }
    }

    public Material hover, idle;
    [SerializeField]
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Transform visualTransform;
    private const float VISUAL_TRANSFORM_DEFAULT_SCALE = 0.1f;

    public void Awake()
    {
        meshRenderer ??= GetComponentInChildren<MeshRenderer>();
        visualTransform ??= GetComponentInChildren<Transform>();
        WorldPosition = transform.position;
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

    public void SetScale(float scale)
    {
        visualTransform.localScale = scale * VISUAL_TRANSFORM_DEFAULT_SCALE * Vector3.one;
    }

    public static bool IsInstance(GameObject gameObject)
    {
        return gameObject.GetComponent<GridElement>() != null;
    }

    public override string ToString()
    {
        return "GridElement(X: " + GridPosition.x + ", Y: " + GridPosition.y + ", isActive:" + gameObject.activeSelf + ", visited: " + Visited + ")";
    }

    public enum Status
    {
        NotVisited = -1,
        Visited = 1,
        Occupied = 0,
    }
}
