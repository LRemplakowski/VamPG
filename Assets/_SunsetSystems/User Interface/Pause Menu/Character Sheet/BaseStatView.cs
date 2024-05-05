using SunsetSystems.UI.Utils;
using SunsetSystems.Utils.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.UI
{
    public class BaseStatView : MonoBehaviour, IUserInterfaceView<BaseStat>
    {
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private List<Image> _chips;
        [SerializeField]
        private Sprite _activeChip, _disabledChip;

        public void UpdateView(IGameDataProvider<BaseStat> dataProvider)
        {
            BaseStat stat = dataProvider.Data;
            _text.text = stat.Name.ToSentenceCase();
            for (int i = 0; i < _chips.Count; i++)
            {
                _chips[i].sprite = i < stat.GetValue() ? _activeChip : _disabledChip;
            }
        }
    }
}
