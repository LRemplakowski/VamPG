using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SunsetSystems.UI
{
    public class SelectGameObjectOnHover : MonoBehaviour, IPointerMoveHandler
    {
        public void OnPointerMove(PointerEventData eventData)
        {
            Selectable selectable = default;
            if (eventData.hovered.Any(go => go.TryGetComponent(out selectable)))
                selectable.Select();
            else
                EventSystem.current.SetSelectedGameObject(null, eventData);
        }
    }
}
