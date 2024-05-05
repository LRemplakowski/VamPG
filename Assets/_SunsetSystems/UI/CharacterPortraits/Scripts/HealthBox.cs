using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterPortraits
{
    [RequireComponent(typeof(Image))]
    public class HealthBox : MonoBehaviour
    {
        [SerializeField]
        private Image image;
        [SerializeField]
        private Sprite healthy, superficial, aggravated;
        [SerializeField, ReadOnly]
        private HealthBoxType _state = HealthBoxType.Healthy;
        internal HealthBoxType State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                UpdateHealthBox();
            }
        }

        private void Start()
        {
            image = GetComponent<Image>();
        }

        private void UpdateHealthBox()
        {
            switch (State)
            {
                case HealthBoxType.Healthy:
                    image.sprite = healthy;
                    break;
                case HealthBoxType.Superficial:
                    image.sprite = superficial;
                    break;
                case HealthBoxType.Aggravated:
                    image.sprite = aggravated;
                    break;
            }
        }
    }
}
