using SunsetSystems.Spellbook;
using SunsetSystems.UI.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SunsetSystems.UI
{
    public class DisciplineGroupUpdateReciever : MonoBehaviour, IUserInterfaceUpdateReciever<BaseStat>
    {
        [SerializeField]
        private DisciplineType _disciplines;
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
            List<Discipline> stats = data
                .Select(s => s.Data as Discipline)
                .Where(d => d.GetValue() > 0 && (d.GetDisciplineType() & _disciplines) > 0)
                .OrderBy(d => d.GetDisciplineType())
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
