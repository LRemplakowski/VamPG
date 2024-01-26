using Sirenix.OdinInspector;
using SunsetSystems.Party;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SunsetSystems.UI
{
    public class CharacterSelector : SerializedMonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _selectedCharacterText;
        private static string _selectedCharacterKey = "";

        [SerializeField]
        private UnityEvent OnSelectedCharacterChanged;
        public static string SelectedCharacterKey 
        { 
            get
            {
                if (string.IsNullOrEmpty(_selectedCharacterKey))
                    _selectedCharacterKey = PartyManager.Instance.MainCharacter.References.CreatureData.DatabaseID;
                return _selectedCharacterKey;
            }
        }

        private void OnEnable()
        {
            UpdateSelectedText();
        }

        private void UpdateSelectedText()
        {
            _selectedCharacterText.text = PartyManager.Instance.GetPartyMemberByID(SelectedCharacterKey).References.CreatureData.FullName;
        }

        public void NextCharacter()
        {
            int currentIndex = PartyManager.Instance.AllCoterieMembers.FindIndex(cd => cd.Equals(SelectedCharacterKey));
            currentIndex = currentIndex + 1 < PartyManager.Instance.AllCoterieMembers.Count ? currentIndex + 1 : 0;
            _selectedCharacterKey = PartyManager.Instance.AllCoterieMembers[currentIndex];
            OnSelectedCharacterChanged?.Invoke();
            UpdateSelectedText();
        }

        public void PreviousCharacter()
        {
            int currentIndex = PartyManager.Instance.AllCoterieMembers.FindIndex(cd => cd.Equals(SelectedCharacterKey));
            currentIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : PartyManager.Instance.AllCoterieMembers.Count - 1;
            _selectedCharacterKey = PartyManager.Instance.AllCoterieMembers[currentIndex];
            OnSelectedCharacterChanged?.Invoke();
            UpdateSelectedText();
        }
    }
}
