using System.Linq;
using Sirenix.OdinInspector;
using SunsetSystems.Core.SceneLoading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SunsetSystems.Persistence.UI
{
    public class SaveLoadScreenManager : MonoBehaviour, ICancelHandler
    {
        [SerializeField, AssetsOnly, Required]
        private SaveEntry _saveEntryPrefab;
        [SerializeField, Required]
        private Transform _saveEntriesParent;
        [SerializeField]
        private GameObject _newSaveGameObject;

        public void ShowScreen(bool includeNewSaveSlot = false)
        {
            gameObject.SetActive(true);
            RefreshSaveScreen(includeNewSaveSlot);
        }

        private void RefreshSaveScreen(bool includeNewSaveSlot)
        {
            if (_newSaveGameObject != null)
            {
                _saveEntriesParent.DestroyChildren(_newSaveGameObject.transform);
                _newSaveGameObject.SetActive(includeNewSaveSlot);
            }
            else
            {
                _saveEntriesParent.DestroyChildren();
            }

            var saveMetaData = SaveLoadManager.GetAllSaveMetaData();
            saveMetaData = saveMetaData.OrderByDescending(save => save.SaveDate);
            foreach (var metaData in saveMetaData)
            {
                SaveEntry saveEntry = Instantiate(_saveEntryPrefab, _saveEntriesParent);
                saveEntry.Initialize(this, metaData);
            }
        }

        public void LoadSave(SaveMetaData saveMetaData)
        {
            if (_newSaveGameObject == null || _newSaveGameObject.activeInHierarchy is false)
                _ = LevelLoader.Instance.LoadSavedGame(saveMetaData.SaveID);
        }

        public void DeleteSave(SaveMetaData saveMetaData)
        {
            SaveLoadManager.DeleteSaveFile(saveMetaData.SaveID);
            RefreshSaveScreen(_newSaveGameObject != null && _newSaveGameObject.activeInHierarchy);
        }

        public void CreateNewSave(string saveName)
        {
            SaveLoadManager.CreateNewSaveFile(saveName);
            RefreshSaveScreen(true);
        }

        public void OnCancel(BaseEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}
