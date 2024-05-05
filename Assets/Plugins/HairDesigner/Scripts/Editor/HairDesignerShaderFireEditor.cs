using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Kalagaan
{
    namespace HairDesignerExtension
    {

        [CustomEditor(typeof(HairDesignerShaderFire))]
        public class HairDesignerShaderFireEditor : HairDesignerShaderEditor
        {

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                if (_destroyed) return;

                HairDesignerShaderFire s = target as HairDesignerShaderFire;

                GUILayout.Label("Render", EditorStyles.boldLabel);
                s._NbFlame = EditorGUILayout.Slider("Details", s._NbFlame, 1, 50);
                s._AlphaTop = EditorGUILayout.Slider("Alpha Top", s._AlphaTop, 0f, 1f);
                s._AlphaBottom = EditorGUILayout.Slider("Alpha Bottom", s._AlphaBottom, 0f, 1f);
                s._Power = EditorGUILayout.Slider("Power", s._Power, 0f, .5f);

                GUILayout.Space(5);
                GUILayout.Label("Motion", EditorStyles.boldLabel);                
                s._Amplitude = EditorGUILayout.Slider("Amplitude", s._Amplitude, 0f, 10f);
                s._Period = EditorGUILayout.Slider("Period", s._Period, 0f, 10f);

                GUILayout.Space(5);
                GUILayout.Label("Color", EditorStyles.boldLabel);
                s._Color1 = EditorGUILayout.ColorField("_Color1", s._Color1);
                s._Color2 = EditorGUILayout.ColorField("_Color2", s._Color2);
                s._ColorWeight = EditorGUILayout.Slider("_ColorWeight", s._ColorWeight, 0, 1);               
            }
        }

    }
}
