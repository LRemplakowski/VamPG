using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GUIDragElementHandler : InputHandler
{
    public UIWindow window;

    private Vector2 offsetFromParent;
    private Canvas canvas;
    private RectTransform rectTransform;

    private void Awake()
    {
        if(window == null)
        {
            window = GetComponentInParent<UIWindow>();
        }
        canvas = FindObjectOfType<Canvas>();
        rectTransform = window.GetComponent<RectTransform>();

        offsetFromParent = window.transform.position - this.transform.position;
    }

    public void OnDrag()
    {
        ManageInput(DragElement);
    }

    private void DragElement()
    {
        Vector2 newPosition = GUIClickController.mousePosition + offsetFromParent;
        Vector2 scale = rectTransform.sizeDelta;
        float xMin = scale.x / 2;
        float xMax = canvas.worldCamera.scaledPixelWidth - scale.x / 2;
        float yMin = scale.y / 2;
        float yMax = canvas.worldCamera.scaledPixelHeight - scale.y / 2;
        newPosition = new Vector2(Mathf.Clamp(newPosition.x, xMin, xMax), Mathf.Clamp(newPosition.y, yMin, yMax));
        window.transform.position = newPosition;
    }
}
