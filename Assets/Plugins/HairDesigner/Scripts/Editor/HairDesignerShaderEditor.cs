using UnityEngine;
using UnityEditor;
using System.Collections;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerShaderEditor : Editor
        {
            public void GUILayoutTextureSlot(string label, ref Texture2D tex, ref Vector2 scale, ref Vector2 offset)
            {

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Label(label);
                scale = EditorGUILayout.Vector2Field("Tiling", scale);
                offset = EditorGUILayout.Vector2Field("Offset", offset);
                GUILayout.EndVertical();

                tex = EditorGUILayout.ObjectField("", tex, typeof(Texture2D), false, GUILayout.MaxWidth(70)) as Texture2D;
                GUILayout.EndHorizontal();
            }


            protected bool _destroyed = false;
            public override void OnInspectorGUI()
            {
                HairDesignerShader s = target as HairDesignerShader;

#if UNITY_2018_3_OR_NEWER
                if (PrefabUtility.IsPartOfPrefabAsset(s))
#else
                if (PrefabUtility.GetPrefabParent(s.gameObject) == null && PrefabUtility.GetPrefabObject(s.gameObject) != null)
#endif
                {
                    GUILayout.Label("Runtime Layer can't be edited", EditorStyles.helpBox);
                    _destroyed = true;
                    return;
                }


                if (s.m_hd == null || s.m_generator==null)
                {
                    if (s.transform.parent != null && s.transform.parent.GetComponent<HairDesignerRuntimeLayerBase>() == null)
                    {
                        //Debug.LogWarning("DestroyImmediate Shader");
                        _destroyed = true;
                        DestroyImmediate(s);
                    }
                }



            }





            public static bool m_copyPast;

            public static bool m_undoRegistered = false;

            public void ShaderGUIBegin<T>(T s) where T : HairDesignerShader
            {
                if (!m_undoRegistered)
                {
                    Event e = Event.current;
                    if ((e.type == EventType.MouseDown && e.button == 0) ||
                        (e.type == EventType.KeyUp && e.keyCode == KeyCode.Tab))
                    {
                        Object o = s as Object;
                        Undo.RegisterCompleteObjectUndo(o, "HairDesigner Shader");
                        m_undoRegistered = true;
                    }
                }


                EditorGUI.BeginChangeCheck();
            }


            
            public void ShaderGUIEnd<T>(T s) where T : HairDesignerShader
            {
                

                
                //Object o = s as Object;

                if (EditorGUI.EndChangeCheck())
                {
                   EditorUtility.SetDirty(s);
                    m_undoRegistered = false;
                }
               


                if(Event.current.control && Event.current.keyCode == KeyCode.Z)
                {
                    //HairDesignerShader sh = s as HairDesignerShader;
                    //if (sh)
                        s.m_generator.m_shaderNeedUpdate = true;
                }


                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy parameters"))
                {
                    m_copyPast = true;
                    //Undo.RecordObject(s, "Shader parameters paste");
                    UnityEditorInternal.ComponentUtility.CopyComponent(s);
                }

                GUI.enabled = m_copyPast;
                if (GUILayout.Button("Paste parameters"))
                {
                    Undo.RecordObject(s, "Shader parameters paste");
                    UnityEditorInternal.ComponentUtility.PasteComponentValues(s);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }


            /*
            static HairDesignerShader m_copy;
            public static void CopyPasteButtons<T>(T s, GameObject go)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy parameters"))
                {
                    //m_copy = go.AddComponent<T>() as HairDesignerShader;
                    //m_copy = new T();
                    m_copy.hideFlags = HideFlags.DontSave | HideFlags.HideInInspector;
                    EditorUtility.CopySerialized(s as UnityEngine.Object, m_copy);
                    DestroyImmediate(m_copy);
                }

                GUI.enabled = m_copy != null;
                if (GUILayout.Button("Paste parameters"))
                {
                    Undo.RecordObject(s, "Shader parameters paste");
                    EditorUtility.CopySerialized(m_copy, s);
                }
                GUILayout.EndHorizontal();
            }
            */
        }

        

    }
}
