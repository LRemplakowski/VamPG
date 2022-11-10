using SunsetSystems.Party;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems
{
    public class CharacterSelector : MonoBehaviour
    {
        private static string _selectedCharacterKey;
        public static string SelectedCharacterKey 
        { 
            get
            {
                if (string.IsNullOrEmpty(_selectedCharacterKey))
                    _selectedCharacterKey = PartyManager.MainCharacter.Data.ID;
                return _selectedCharacterKey;
            }
        }
    }
}
