using SunsetSystems.Party;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunsetSystems
{
    public class CharacterSelector : MonoBehaviour
    {
        private static string _selectedCharacterKey = default;
        public static string SelectedCharacterKey 
        { 
            get
            {
                if (string.IsNullOrEmpty(_selectedCharacterKey))
                    _selectedCharacterKey = PartyManager.ActiveParty[0].Data.ID;
                return _selectedCharacterKey;
            }
        }
    }
}
