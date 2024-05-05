using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Kalagaan.HairDesignerExtension;

public class HairDesigner_DebugLayerTool : EditorWindow
{

    HairDesigner m_hdComponent;


    [MenuItem("Window/HairDesigner/Debug Layer")]
    static void Init()
    {     
        HairDesigner_DebugLayerTool window = (HairDesigner_DebugLayerTool)EditorWindow.GetWindow(typeof(HairDesigner_DebugLayerTool));
        window.Show();
    }


    

    void OnGUI()
    {
        if (m_hdComponent == null)
        {
            GUILayout.Label("Select the hairDesigner gameObject", EditorStyles.boldLabel);            
        }
     
        
        if( Selection.activeGameObject != null )
        {
            if (m_hdComponent == null || Selection.activeGameObject != m_hdComponent.gameObject)
                m_hdComponent = Selection.activeGameObject.GetComponent<HairDesigner>();
        }

        if (m_hdComponent == null)
            return;


        HairDesignerGenerator[] generators = m_hdComponent.GetComponents<HairDesignerGenerator>();

       
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Layer list", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();        
        GUILayout.EndHorizontal();


        if (generators == null || generators.Length == 0)
        {
            GUILayout.Label("No generator detected");
        }
        else
        {
            for (int i = 0; i < generators.Length; ++i)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label(generators[i].m_name);
                GUILayout.FlexibleSpace();
                GUILayout.Label(generators[i].m_layerType.ToString());
                GUILayout.EndHorizontal();
            }
        }

        GUILayout.EndVertical();
       


        HairDesignerShader[] shadersCtrls = m_hdComponent.GetComponents<HairDesignerShader>();

        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Shader controller list", EditorStyles.boldLabel);

        int deleteId = -1;

        if (shadersCtrls == null || shadersCtrls.Length == 0 )
        {
            GUILayout.Label("No Shader controller detected");
        }
        else
        {

            for (int i = 0; i < shadersCtrls.Length; ++i)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                if (shadersCtrls[i].m_generator != null)
                {
                    GUILayout.Label(shadersCtrls[i].m_generator.m_name);
                }
                else
                {
                    GUI.color = Color.red;
                    GUILayout.Label("Undefined layer ");
                    if (GUILayout.Button("Delete"))
                    {
                        deleteId = i;
                    }
                }
                GUI.color = Color.white;

                GUILayout.FlexibleSpace();
                GUILayout.Label(shadersCtrls[i].m_shader.name);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            if (deleteId != -1)
            {
                DestroyImmediate(shadersCtrls[deleteId]);
                return;
            }
        }
    }


    
}
