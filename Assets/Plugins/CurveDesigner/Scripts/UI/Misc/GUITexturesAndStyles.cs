using UnityEngine;

namespace ChaseMacMillan.CurveDesigner
{
    public class GUITexturesAndStyles : ScriptableObject
    {
        public Texture2D lineTex;
        public Texture2D circleIcon;
        public Texture2D squareIcon;
        public Texture2D diamondIcon;
        public Texture2D plusButton;
        public Texture2D uiIcon;
        public Texture2D uiArea;
        public GUIStyle modeSelectorStyle;
        public GUIStyle selectorWindowStyle;
        public GUIStyle colorPickerBoxStyle;
        [Range(.1f,3f)]
        public float buttonSizeScalar = 1;

        private void OnValidate()
        {
            if (modeSelectorStyle.border == null)
                modeSelectorStyle.border = new();
            if (modeSelectorStyle.margin == null)
                modeSelectorStyle.margin = new();
            if (modeSelectorStyle.padding == null)
                modeSelectorStyle.padding = new();
            if (modeSelectorStyle.overflow == null)
                modeSelectorStyle.overflow = new();

            if (selectorWindowStyle.border == null)
                selectorWindowStyle.border = new();
            if (selectorWindowStyle.margin == null)
                selectorWindowStyle.margin = new();
            if (selectorWindowStyle.padding == null)
                selectorWindowStyle.padding = new();
            if (selectorWindowStyle.overflow == null)
                selectorWindowStyle.overflow = new();

            if (colorPickerBoxStyle.border == null)
                colorPickerBoxStyle.border = new();
            if (colorPickerBoxStyle.margin == null)
                colorPickerBoxStyle.margin = new();
            if (colorPickerBoxStyle.padding == null)
                colorPickerBoxStyle.padding = new();
            if (colorPickerBoxStyle.overflow == null)
                colorPickerBoxStyle.overflow = new();
        }
    }
}
