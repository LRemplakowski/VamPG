using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Kalagaan.HairDesignerExtension
{
    [CustomEditor(typeof(HairDesignerStrandMeshCollectionBase),true)]
    public class HairDesignerStrandMeshCollectionEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            HairDesignerStrandMeshCollectionBase smc = target as HairDesignerStrandMeshCollectionBase;
            
            for ( int i=0; i< smc.m_collection.Count; ++i )
            {
                if (smc.m_collection[i] == null)
                    continue;
                GUILayout.BeginVertical(EditorStyles.helpBox);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Name");
                smc.m_collection[i].name = GUILayout.TextField(smc.m_collection[i].name, GUILayout.MinWidth(100));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("-"))
                {
                    smc.m_collection.RemoveAt(i--);
                    continue;
                }
                GUILayout.EndHorizontal();

                GUI.enabled = false;
                //GUILayout.Label("" + smc.m_collection[i].id);
                GUI.enabled = true;
                smc.m_collection[i].mesh = EditorGUILayout.ObjectField("Mesh", smc.m_collection[i].mesh, typeof(Mesh), false) as Mesh;

                if(smc.m_collection[i].mesh !=null)
                {
                    if(!smc.m_collection[i].mesh.isReadable)
                    {
                        EditorGUILayout.HelpBox("The mesh is not readable.\nPlease edit the import settings.", MessageType.Error);
                    }
                }

                smc.m_collection[i].orientation = EditorGUILayout.Vector3Field("Orientation", smc.m_collection[i].orientation);
                GUILayout.EndVertical();                
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("  +  "))
            {
                HairDesignerStrandMeshCollectionBase.StrandMesh sm = new HairDesignerStrandMeshCollectionBase.StrandMesh();
                sm.id = smc.CreateNewID();

                smc.m_collection.Add(sm);
            }
            GUILayout.EndHorizontal();

            
            //base.DrawDefaultInspector();
        }


        public void OnDestroy()
        {
            HairDesignerStrandMeshCollectionBase smc = target as HairDesignerStrandMeshCollectionBase;
            EditorUtility.SetDirty(smc);
            AssetDatabase.SaveAssets();
        }
    }
}