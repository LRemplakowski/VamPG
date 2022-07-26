using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunsetSystems.Utils.UI
{
    public static class InputHelper
    {
        public static bool IsRaycastHittingUIObject(Vector2 position)
        {
            if (m_PointerData == null)
                m_PointerData = new PointerEventData(EventSystem.current);
            m_PointerData.position = position;
            EventSystem.current.RaycastAll(m_PointerData, m_RaycastResults);
            return m_RaycastResults.Count > 0;
        }

        private static PointerEventData m_PointerData;
        private static List<RaycastResult> m_RaycastResults = new();
    }
}