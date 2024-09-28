using Sirenix.OdinInspector;
using SunsetSystems.Utils.ObjectPooling;
using UnityEngine;

namespace SunsetSystems.Tooltips
{
    public abstract class AbstractTooltip<T> : SerializedMonoBehaviour, ITooltip<T>, IPooledObject where T : ITooltipContext
    {
        [Title("Tooltip Common")]
        [SerializeField]
        private RectTransform _myRect;
        [Title("Tooltip Common Runtime")]
        [ShowInInspector, ReadOnly]
        private RectTransform _parentRect;
        [ShowInInspector, ReadOnly]
        protected T _contextReference;
        [ShowInInspector, ReadOnly]
        private bool _alwaysUpdatePosition;
        [ShowInInspector, ReadOnly]
        private bool _convertPositionToCanvasSpace;

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
            if (_convertPositionToCanvasSpace)
            {
                var screenPoint = Camera.main.WorldToScreenPoint(_contextReference.TooltipPosition);
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_parentRect, screenPoint, null, out Vector2 localPoint))
                    _myRect.localPosition = localPoint;
            }
            else
            {
                _myRect.position = _contextReference.TooltipPosition;
            }
        }

        public void SetConvertPositionToCanvasSpace(bool convert)
        {
            _convertPositionToCanvasSpace = convert;
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
