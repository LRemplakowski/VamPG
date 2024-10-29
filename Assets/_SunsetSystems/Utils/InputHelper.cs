using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunsetSystems.Utils.Input
{
    public static class InputHelper
    {
        public static bool IsRaycastHittingUIObject(Vector2 position, out List<RaycastResult> results)
        {
            if (m_PointerData == null)
                m_PointerData = new PointerEventData(EventSystem.current);
            m_PointerData.position = position;
            EventSystem.current?.RaycastAll(m_PointerData, m_RaycastResults);
            results = m_RaycastResults;
            return m_RaycastResults.Count > 0;
        }

        public static bool DoesAnyUIHitBlockRaycasts(List<RaycastResult> hits)
        {
            return hits.Any((hit) => { var cg = hit.gameObject.GetComponentInParent<CanvasGroup>(); return cg != null && cg.blocksRaycasts; });
        }

        private static PointerEventData m_PointerData;
        private static List<RaycastResult> m_RaycastResults = new();
    }
}