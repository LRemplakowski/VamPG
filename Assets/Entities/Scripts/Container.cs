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
    public float InteractionDistance { get; set; }

    public void Interact()
    {
        throw new System.NotImplementedException();
    }
}
