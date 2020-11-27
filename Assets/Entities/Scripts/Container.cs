using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Entity, IInteractable
{
    [HideInInspector, SerializeField] 
    private GameObject _hoverHighlight;
    [ExposeProperty]
    public GameObject HoverHighlight 
    { 
        get => _hoverHighlight; 
        set => _hoverHighlight = value; 
    }

    private bool _isHoveredOver;
    [HideInInspector]
    public bool IsHoveredOver
    {
        get => _isHoveredOver;
        set
        {
            _isHoveredOver = value;
            HoverHighlight.SetActive(IsHoveredOver);
        }
    }

    [HideInInspector, SerializeField]
    private float _interactionDistance = 1.0f;
    [ExposeProperty]
    public float InteractionDistance 
    { 
        get => _interactionDistance;
        set => _interactionDistance = value; 
    }
    public GameObject TargetedBy { get; set; }
    public bool Interacted { get; set; }

    [HideInInspector, SerializeField]
    private Transform _interactionTarget;
    [ExposeProperty]
    public Transform InteractionTarget 
    { 
        get => _interactionTarget;
        set => _interactionTarget = value; 
    }

    public void Awake()
    {
        if(InteractionTarget == null)
        {
            InteractionTarget = this.transform;
        }
    }

    public void Interact()
    {
        Debug.Log(TargetedBy + " interacted with object " + gameObject);
        Interacted = true;
        TargetedBy = null;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactionDistance);
    }

    public Transform GetTransform()
    {
        return this.transform;
    }
}
