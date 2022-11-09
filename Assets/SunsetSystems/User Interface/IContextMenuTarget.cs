using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunsetSystems.UI
{
    public interface IContextMenuTarget : IPointerClickHandler
    {
        void OpentContextMenu();
    }
}
