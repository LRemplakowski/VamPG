using Sirenix.OdinInspector;
using Redcode.Awaiting;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SunsetSystems.UI
{
    [System.Serializable]
    public class GameTooltip : MonoBehaviour, ITooltip
    {
        [SerializeField, Required]
        private TextMeshProUGUI _title, _description;
        [SerializeField, Required]
        private CanvasGroup _canvasGroup;
        [SerializeField, Range(0, 3)]
        private float _fadeTime = 1f;

        private bool _showTooltip;

        public void ShowTooltip(Transform parent, TooltipContent content)
        {
            _showTooltip = true;
            transform.SetParent(parent, false);
            _ = FadeInTooltip();
        }

        private async Task FadeInTooltip()
        {
            float lerp = 0f;
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(true);
            while (_showTooltip && _canvasGroup.alpha < 1f)
            {
                lerp += Time.deltaTime / _fadeTime;
                _canvasGroup.alpha = lerp;
                await new WaitForUpdate();
            }
        }

        public void HideTooltip()
        {
            _showTooltip = false;
            _ = FadeOutTooltip();
        }

        private async Task FadeOutTooltip()
        {
            float lerp = 1f;
            _canvasGroup.alpha = 1f;
            while (_showTooltip == false && _canvasGroup.alpha > 0f)
            {
                lerp -= Time.deltaTime / _fadeTime;
                _canvasGroup.alpha = lerp;
                await new WaitForUpdate();
            }
            if (_showTooltip == false)
                gameObject.SetActive(false);
        }
    }

    public interface ITooltip
    {
        void ShowTooltip(Transform parent, TooltipContent content);

        void HideTooltip();
    }

    [System.Serializable]
    public struct TooltipContent
    {
        public string _title;
        [MultiLineProperty]
        public string _description;
    }
}
