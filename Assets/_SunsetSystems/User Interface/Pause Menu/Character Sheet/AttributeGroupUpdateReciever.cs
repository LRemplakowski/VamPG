using NaughtyAttributes;
using SunsetSystems.Entities.Characters;
using SunsetSystems.UI.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class AttributeGroupUpdateReciever : MonoBehaviour, IUserInterfaceUpdateReciever<BaseStat>
    {
        [SerializeField, EnumFlags]
        private AttributeType _attributes;
        [SerializeField]
        private List<BaseStatView> _views = new();
        [SerializeField]
        private Transform _viewsParent;
        [SerializeField]
        private BaseStatView _viewPrefab;

        public void DisableViews()
        {
            _views.ForEach(v => v.gameObject.SetActive(false));
        }

        public void UpdateViews(List<IGameDataProvider<BaseStat>> data)
        {
            DisableViews();
            List<CreatureAttribute> stats = data
                .FindAll(a => ((a as CreatureAttribute).AttributeType & _attributes) > 0)
                .OrderBy(a => (a as CreatureAttribute).AttributeType)
                .Select(a => a.Data as CreatureAttribute)
                .ToList();
            foreach (BaseStat stat in stats)
            {
                BaseStatView view = GetView();
                view.UpdateView(stat);
                view.gameObject.SetActive(true);
            }
        }

        public BaseStatView GetView()
        {
            BaseStatView view;
            view = _views.FirstOrDefault(v => v.isActiveAndEnabled == false);
            if (view == null)
            {
                view = Instantiate(_viewPrefab, _viewsParent);
                view.gameObject.SetActive(false);
                _views.Add(view);
            }
            return view;
        }
    }
}
