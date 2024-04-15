using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SunsetSystems.Persistence.UI
{
    public class SaveEntry : SerializedMonoBehaviour
    {
        [Title("References")]
        [SerializeField]
        private Image _saveImage;
        [SerializeField]
        private TextMeshProUGUI _saveName, _saveDate;

        [Title("Runtime")]
        [ShowInInspector, ReadOnly]
        private SaveLoadScreenManager _saveScreenManager;
        [ShowInInspector, ReadOnly]
        private SaveMetaData _saveMeta;

        public void Initialize(SaveLoadScreenManager manager, SaveMetaData metaData)
        {
            _saveMeta = metaData;
            _saveName.text = metaData.SaveName;
            _saveDate.text = GetFormattedSaveDate(metaData.SaveDate);
            _saveScreenManager = manager;
        }

        private string GetFormattedSaveDate(string dateString)
        {
            return dateString;
        }

        [Button]
        public void LoadSave()
        {
            _saveScreenManager.LoadSave(_saveMeta);
        }

        [Button]
        public void DeleteSave()
        {
            _saveScreenManager.DeleteSave(_saveMeta);
        }
    }
}
