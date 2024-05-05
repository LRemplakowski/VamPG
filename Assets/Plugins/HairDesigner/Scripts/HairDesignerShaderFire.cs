using UnityEngine;
using System.Collections;


namespace Kalagaan
{
    namespace HairDesignerExtension
    {
        public class HairDesignerShaderFire : HairDesignerShader
        {
            public float _NbFlame = 5f;
            public float _AlphaTop = .5f;
            public float _AlphaBottom = .3f;
            public float _Power = .05f;
            public float _Amplitude = 2f;
            public float _Period = 1f;
            public Color _Color1 = Color.yellow;
            public Color _Color2 = Color.red;
            public float _ColorWeight = .3f;

            public override void UpdatePropertyBlock(ref MaterialPropertyBlock pb, HairDesignerBase.eLayerType lt)
            {                
                pb.SetColor("_Color1", _Color1);
                pb.SetColor("_Color2", _Color2);               
                pb.SetFloat("_NbFlame", _NbFlame);
                pb.SetFloat("_AlphaTop", _AlphaTop);
                pb.SetFloat("_AlphaBottom", _AlphaBottom);
                pb.SetFloat("_Power", _Power);
                pb.SetFloat("_Amplitude", _Amplitude);
                pb.SetFloat("_Period", _Period);
                pb.SetFloat("_ColorWeight", _ColorWeight);                             
            }
        }
    }
}
