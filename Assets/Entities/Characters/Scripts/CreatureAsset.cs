using UnityEngine;
using UMA.CharacterSystem;
using System;

namespace Entities.Characters
{
    [CreateAssetMenu(fileName = "New Creature Asset", menuName = "Character/Creature Asset")]
    public class CreatureAsset : ScriptableObject
    {
        [SerializeField]
        private string _creatureName = "New Creature";
        public string CreatureName { get => _creatureName; internal set => _creatureName = value; }
        [SerializeField]
        private string _creatureLastName = "";
        public string CreatureLastName { get => _creatureLastName; internal set => _creatureLastName = value; }
        [SerializeField]
        private Sprite _portrait;
        public Sprite Portrait { get => _portrait; internal set => _portrait = value; }
        [SerializeField]
        private CharacterStats _statsAsset;
        public CharacterStats StatsAsset { get => _statsAsset; internal set => _statsAsset = value; }
        [SerializeField]
        private string _umaPresetFilename = "default";
        public string UmaPresetFilename { get => _umaPresetFilename; internal set => _umaPresetFilename = value; }
        [SerializeField]
        private RuntimeAnimatorController _animatorController;
        public RuntimeAnimatorController AnimatorController { get => _animatorController; internal set => _animatorController = value; }
        [SerializeField]
        private Faction _creatureFaction;
        public Faction CreatureFaction { get => _creatureFaction; internal set => _creatureFaction = value; }
        [SerializeField]
        private BodyType _bodyType;
        public BodyType BodyType { get => _bodyType; internal set => _bodyType = value; }
        [SerializeField]
        private CreatureType _creatureType;
        public CreatureType CreatureType { get => _creatureType; internal set => _creatureType = value; }


        private void OnEnable()
        {
            if (_portrait == null)
                _portrait = Resources.Load<Sprite>("DEBUG/missing");
            if (_statsAsset == null)
                _statsAsset = Resources.Load<CharacterStats>("DEBUG/DebugStats");
            if (_animatorController == null)
                _animatorController = Resources.Load<RuntimeAnimatorController>("Animation/AnimationControllers/female_anims");
        }

        internal static CreatureAsset CopyInstance(CreatureAsset data)
        {
            CreatureAsset copy = CreateInstance<CreatureAsset>();
            copy._creatureName = data._creatureName;
            copy._creatureLastName = data._creatureLastName;
            copy._portrait = data._portrait;
            copy._statsAsset = CharacterStats.CopyAssetInstance(data._statsAsset);
            copy._umaPresetFilename = data._umaPresetFilename;
            copy._animatorController = data._animatorController;
            copy._creatureFaction = data._creatureFaction;
            copy._bodyType = data._bodyType;
            copy._creatureType = data._creatureType;
            return copy;
        }
    }
}
