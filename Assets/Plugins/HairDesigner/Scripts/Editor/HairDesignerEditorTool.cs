using System;
using UnityEngine;
using UnityEditor;


#if UNITY_2019_1_OR_NEWER
using UnityEditor.EditorTools;
namespace Kalagaan.HairDesignerExtension
{

    [EditorTool("HairDesigner tool", typeof(Kalagaan.HairDesignerExtension.HairDesignerBase))]
    class HairDesignerEditorTool : EditorTool
    {
        static Texture2D m_ToolIcon;

        GUIContent m_IconContent;

        void OnEnable()
        {
            if (m_ToolIcon == null)
            {
                m_ToolIcon = (Texture2D)Resources.Load("Icons/TOOL", typeof(Texture2D));
            }

            m_IconContent = new GUIContent()
            {
                image = m_ToolIcon,
                text = "HairDesigner Tool",
                tooltip = "HairDesigner Tool"
            };
        }

        public override GUIContent toolbarIcon
        {
            get { return m_IconContent; }
        }


        public override void OnToolGUI(EditorWindow window)
        {

            if (!SceneView.lastActiveSceneView.drawGizmos)
            {
                if (HairDesignerEditor.m_EditorInstance != null)
                {
                    HairDesignerEditor.m_EditorInstance.DrawSceneGUI();
                }

            }
        }
    }
}

#endif
