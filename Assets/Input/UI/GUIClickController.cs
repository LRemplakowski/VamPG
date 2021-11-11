using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class GUIClickController : InputHandler, IPointerClickHandler, IPointerDownHandler, IDragHandler
{
    public UnityEvent onLeft;
    public UnityEvent onRightHold;
    public UnityEvent onDrag;

    public static Vector2 mousePosition;

    public override void Initialize()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            ManageInput(InvokeDrag, eventData);
        }
    }

    private void InvokeDrag(PointerEventData eventData)
    {
        mousePosition = eventData.position;
        InvokeEvent(onDrag);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !eventData.dragging)
        {
            ManageInput(InvokeEvent, onLeft);
        }
    }

    private void InvokeEvent(UnityEvent e)
    {
        e.Invoke();
    }

    //Used to handle mouse button hold
    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            ManageInput(InvokeEvent, onRightHold);
        }
    }
}