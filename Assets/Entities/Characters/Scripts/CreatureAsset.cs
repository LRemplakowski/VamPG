using UnityEngine;
using UMA.CharacterSystem;

namespace Entities.Characters
{
    [CreateAssetMenu(fileName = "New Creature Asset", menuName = "Character/Creature Asset")]
    public class CreatureAsset : ScriptableObject
    {
        [SerializeField]
        private string _creatureName = "New Creature";
        public string CreatureName { get => _creatureName; }
        [SerializeField]
        private Sprite _portrait;
        public Sprite Portrait { get => _portrait; }
        [SerializeField]
        private CharacterStats _statsAsset;
        public CharacterStats StatsAsset { get => _statsAsset; }
        [SerializeField]
        private string _umaPresetFilename = "default";
        public string UmaPresetFilename { get => _umaPresetFilename; }
        [SerializeField]
        private RuntimeAnimatorController _animatorController;
        public RuntimeAnimatorController AnimatorController { get => _animatorController; }
        

        private void OnEnable()
        {
            if (_portrait == null)
                _portrait = Resources.Load<Sprite>("DEBUG/missing");
            if (_statsAsset == null)
                _statsAsset = Resources.Load<CharacterStats>("DEBUG/DebugStats");
            if (_animatorController == null)
                _animatorController = Resources.Load<RuntimeAnimatorController>("Animation/AnimationControllers/female_anims");
        }
    }
}
