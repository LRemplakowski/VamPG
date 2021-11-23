using UnityEngine;
using UMA.CharacterSystem;

namespace Entities.Characters
{
    [CreateAssetMenu(fileName = "New Creature Asset", menuName = "Character/Creature Asset")]
    public class CreatureAsset : ScriptableObject
    {
        [SerializeField]
        private string creatureName;
        [SerializeField]
        private Sprite portrait;
        [SerializeField]
        private CharacterStats statsAsset;


    }
}
