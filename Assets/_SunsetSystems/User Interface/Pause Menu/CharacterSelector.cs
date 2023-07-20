using SunsetSystems.Party;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SunsetSystems
{
    public class CharacterSelector : MonoBehaviour
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
                    _selectedCharacterKey = PartyManager.MainCharacter.ID;
                return _selectedCharacterKey;
            }
        }

        private void OnEnable()
        {
            UpdateSelectedText();
        }

        private void UpdateSelectedText()
        {
            _selectedCharacterText.text = PartyManager.Instance.GetPartyMemberByID(SelectedCharacterKey).Name;
        }

        public void NextCharacter()
        {
            int currentIndex = PartyManager.AllCoterieMembers.FindIndex(cd => cd.ID.Equals(SelectedCharacterKey));
            currentIndex = currentIndex + 1 < PartyManager.AllCoterieMembers.Count ? currentIndex + 1 : 0;
            _selectedCharacterKey = PartyManager.AllCoterieMembers[currentIndex].ID;
            OnSelectedCharacterChanged?.Invoke();
            UpdateSelectedText();
        }

        public void PreviousCharacter()
        {
            int currentIndex = PartyManager.AllCoterieMembers.FindIndex(cd => cd.ID.Equals(SelectedCharacterKey));
            currentIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : PartyManager.AllCoterieMembers.Count - 1;
            _selectedCharacterKey = PartyManager.AllCoterieMembers[currentIndex].ID;
            OnSelectedCharacterChanged?.Invoke();
            UpdateSelectedText();
        }
    }
}
