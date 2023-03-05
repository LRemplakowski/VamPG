using SunsetSystems.Party;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace SunsetSystems.UI
{
    public class CharacterSelector : MonoBehaviour, ICharacterSelector
    {
        [SerializeField]
        private TextMeshProUGUI _selectedCharacterText;
        private string _selectedCharacterKey = default;

        public UnityEvent OnSelectedCharacterChanged;
        public string SelectedCharacterKey 
        { 
            get
            {
                if (string.IsNullOrEmpty(_selectedCharacterKey))
                    _selectedCharacterKey = _partyManager.MainCharacter.Data.ID;
                return _selectedCharacterKey;
            }
        }

        private IPartyManager _partyManager;

        [Inject]
        public void InjectDependencies(IPartyManager partyManager)
        {
            _partyManager = partyManager;
        }

        private void OnEnable()
        {
            UpdateSelectedText();
        }

        private void UpdateSelectedText()
        {
            _selectedCharacterText.text = _partyManager.GetPartyMemberByID(_selectedCharacterKey).Data.FullName;
        }

        public void NextCharacter()
        {
            int currentIndex = _partyManager.AllCoterieMembers.FindIndex(cd => cd.ID.Equals(SelectedCharacterKey));
            currentIndex = currentIndex + 1 < _partyManager.AllCoterieMembers.Count ? currentIndex + 1 : 0;
            _selectedCharacterKey = _partyManager.AllCoterieMembers[currentIndex].ID;
            OnSelectedCharacterChanged?.Invoke();
            UpdateSelectedText();
        }

        public void PreviousCharacter()
        {
            int currentIndex = _partyManager.AllCoterieMembers.FindIndex(cd => cd.ID.Equals(SelectedCharacterKey));
            currentIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : _partyManager.AllCoterieMembers.Count - 1;
            _selectedCharacterKey = _partyManager.AllCoterieMembers[currentIndex].ID;
            OnSelectedCharacterChanged?.Invoke();
            UpdateSelectedText();
        }
    }
}
