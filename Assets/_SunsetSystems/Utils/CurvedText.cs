using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SunsetSystems.Utils
{
    [ExecuteAlways]
    [RequireComponent(typeof(TextMeshPro))]
    public class CurvedText : MonoBehaviour
    {
        public float Radius = 10f;
        public TextMeshPro textMeshPro;
        public TMP_TextInfo textInfo;

        private void Awake()
        {
            if (textMeshPro == null)
                textMeshPro = GetComponent<TextMeshPro>();
        }

        private void Start()
        {
            if (textMeshPro == null)
                textMeshPro = GetComponent<TextMeshPro>();
            textInfo = textMeshPro.textInfo;
            CurveText();
        }

        private void Update()
        {
            CurveText();
        }

        [Button]
        void CurveText()
        {
            textMeshPro.ForceMeshUpdate();
            textInfo = textMeshPro.textInfo;

            int characterCount = textInfo.characterCount;
            if (characterCount == 0) return;

            // Calculate the index of the middle character
            int middleIndex = characterCount / 2;

            // Determine the center position relative to the middle character
            Vector3 middleCharPosition = textInfo.characterInfo[middleIndex].bottomLeft +
            (textInfo.characterInfo[middleIndex].topRight - textInfo.characterInfo[middleIndex].bottomLeft) / 2;

            for (int i = 0; i < characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible)
                    continue;

                // Calculate the angle for this character
                float angle = Mathf.Deg2Rad * ((360f / characterCount) * (i - middleIndex));
                Vector3 offset = new Vector3(Mathf.Cos(angle) * Radius, Mathf.Sin(angle) * Radius, 0);

                for (int j = 0; j < 4; j++)
                {
                    Vector3 charOffset = textInfo.characterInfo[i].vertex_BL.position + new Vector3(0.5f, 0.5f, 0);
                    textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices[textInfo.characterInfo[i].vertexIndex + j] += offset - middleCharPosition;
                }
            }
            textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }

    }
}
