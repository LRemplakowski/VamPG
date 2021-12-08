using UnityEngine;
using UMA.CharacterSystem;
using System;
using Utils.ResourceLoader;

namespace Entities.Characters
{
    [CreateAssetMenu(fileName = "New Creature Asset", menuName = "Character/Creature Asset")]
    public class CreatureAsset : ScriptableObject
    {
        [SerializeField]
        private string _creatureName = "New Creature";
        public string CreatureName { get => _creatureName; set => _creatureName = value; }
        [SerializeField]
        private string _creatureLastName = "";
        public string CreatureLastName { get => _creatureLastName; set => _creatureLastName = value; }
        [SerializeField]
        private Sprite _portrait;
        public Sprite Portrait { get => _portrait; set => _portrait = value; }
        [SerializeField]
        private CharacterStats _statsAsset;
        public CharacterStats StatsAsset { get => _statsAsset; set => _statsAsset = value; }
        [SerializeField]
        private string _umaPresetFilename = "default";
        public string UmaPresetFilename { get => _umaPresetFilename; set => _umaPresetFilename = value; }
        [SerializeField]
        private string animatorControllerResourceName = "";
        private RuntimeAnimatorController _animatorController;
        public RuntimeAnimatorController AnimatorController 
        { 
            get => _animatorController; 
        }
        [SerializeField]
        private Faction _creatureFaction;
        public Faction CreatureFaction { get => _creatureFaction; set => _creatureFaction = value; }
        [SerializeField]
        private BodyType _bodyType;
        public BodyType BodyType { get => _bodyType; set => _bodyType = value; }
        [SerializeField]
        private CreatureType _creatureType;
        public CreatureType CreatureType { get => _creatureType; set => _creatureType = value; }


        private void OnEnable()
        {
            if (_portrait == null)
                _portrait = Resources.Load<Sprite>("DEBUG/missing");
            if (_statsAsset == null)
                _statsAsset = Resources.Load<CharacterStats>("DEBUG/DebugStats");
            if (_animatorController == null)
                _animatorController = Resources.Load<RuntimeAnimatorController>("Animation/AnimationControllers/female_anims");
        }

        private void Awake()
        {
            if (_animatorController == null)
                _animatorController = ResourceLoader.GetAnimatorController(animatorControllerResourceName);
        }

        internal static CreatureAsset CopyInstance(CreatureAsset data)
        { 
            CreatureAsset copy = CreateInstance<CreatureAsset>();
            copy.name = data.name + " (Clone)";
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
