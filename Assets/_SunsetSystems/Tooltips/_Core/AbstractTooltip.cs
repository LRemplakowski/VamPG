using Sirenix.OdinInspector;
using SunsetSystems.Utils.ObjectPooling;
using UnityEngine;

namespace SunsetSystems.Tooltips
{
    public abstract class AbstractTooltip<T> : SerializedMonoBehaviour, ITooltip<T>, IPooledObject where T : ITooltipContext
    {
        [Title("Tooltip Common")]
        [SerializeField]
        private bool _alwaysUpdatePosition;
        [SerializeField]
        private RectTransform _myRect;
        [ShowInInspector, ReadOnly]
        private RectTransform _parentRect;
        [ShowInInspector, ReadOnly]
        protected T _contextReference;

        private void Update()
        {
            if (_alwaysUpdatePosition)
                UpdateTooltipPosition();
        }

        #region IPooledObject
        public void ResetObject()
        {
            DoCleanUp();
            _contextReference = default;
            _alwaysUpdatePosition = default;
        }
        #endregion

        #region ITooltip
        public bool InjectTooltipData(ITooltipContext context)
        {
            if (context is T typedContext)
                return InjectTooltipData(typedContext);
            return false;
        }

        public void RefreshTooltip()
        {
            UpdateTooltipFromContext(_contextReference);
        }

        public bool InjectTooltipData(T context)
        {
            if (context == null)
            {
                Debug.LogWarning($"Tooltip {name} recieved a null context!");
                return false;
            }
            _contextReference = context;
            RefreshTooltip();
            return true;
        }

        public void UpdateTooltipPosition()
        {
            var screenPoint = Camera.main.WorldToScreenPoint(_contextReference.TooltipPosition);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, screenPoint, null, out Vector2 localPoint))
                _myRect.localPosition = localPoint;
        }

        public void SetAlwaysUpdatePosition(bool update)
        {
            _alwaysUpdatePosition = update;
        }

        public void SetParentTransfrom(RectTransform parent)
        {
            _parentRect = parent;
            transform.SetParent(parent);
        }
        #endregion

        #region Tooltip Implementation
        protected abstract void UpdateTooltipFromContext(T context);
        protected abstract void DoCleanUp();
        #endregion
    }
}
