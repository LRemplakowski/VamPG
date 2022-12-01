using SunsetSystems.Party;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SunsetSystems
{
    public class CharacterSelector : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _selectedCharacterText;
        private static string _selectedCharacterKey = default;
        public static string SelectedCharacterKey 
        { 
            get
            {
                if (string.IsNullOrEmpty(_selectedCharacterKey))
                    _selectedCharacterKey = PartyManager.MainCharacter.Data.ID;
                return _selectedCharacterKey;
            }
        }

        private void OnEnable()
        {
            _selectedCharacterKey = PartyManager.MainCharacter.Data.FullName;
        }

        private void UpdateSelectedText()
        {
            _selectedCharacterKey = PartyManager.Instance.GetPartyMemberByID(_selectedCharacterKey).Data.FullName;
        }

        public void NextCharacter()
        {
            int currentIndex = PartyManager.AllCoterieMembers.FindIndex(cd => cd.ID.Equals(SelectedCharacterKey));
            currentIndex = currentIndex + 1 < PartyManager.AllCoterieMembers.Count ? currentIndex + 1 : 0;
            _selectedCharacterKey = PartyManager.AllCoterieMembers[currentIndex].ID;
            UpdateSelectedText();
        }

        public void PreviousCharacter()
        {
            int currentIndex = PartyManager.AllCoterieMembers.FindIndex(cd => cd.ID.Equals(SelectedCharacterKey));
            currentIndex = currentIndex - 1 >= 0 ? currentIndex - 1 : PartyManager.AllCoterieMembers.Count - 1;
            _selectedCharacterKey = PartyManager.AllCoterieMembers[currentIndex].ID;
            UpdateSelectedText();
        }
    }
}
