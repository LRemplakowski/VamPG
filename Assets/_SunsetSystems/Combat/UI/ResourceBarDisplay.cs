using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Combat.UI
{
    public class ResourceBarDisplay : SerializedMonoBehaviour
    {
        [SerializeField]
        private List<Image> activeChunks = new();
        [SerializeField, Min(0f)]
        private float chunkStateLerpTime = 1f;

        private IEnumerator lerpChunksCoroutine;

        public void UpdateActiveChunks(int activeCount)
        {
            if (lerpChunksCoroutine != null)
                StopCoroutine(lerpChunksCoroutine);
            lerpChunksCoroutine = LerpChunksActive(activeCount, chunkStateLerpTime);
            StartCoroutine(lerpChunksCoroutine);
        }

        private IEnumerator LerpChunksActive(int activeCount, float timePerChunk)
        {
            for(int i = 0; i < activeChunks.Count; i++)
            {
                Image chunk = activeChunks[i];
                float chunkAlpha = chunk.color.a;
                float targetAlpha = i <= activeCount ? 1f : 0f;
                if (Mathf.Approximately(chunkAlpha, targetAlpha))
                {
                    chunk.CrossFadeAlpha(targetAlpha, 0f, false);
                    continue;
                }
                else
                {
                    chunk.CrossFadeAlpha(targetAlpha, timePerChunk, false);
                    yield return new WaitForSeconds(timePerChunk);
                }
            }
            lerpChunksCoroutine = null;
        }
    }
}
