using UnityEngine;
using NaughtyAttributes;
using SunsetSystems.Entities.Data;
using SunsetSystems.Resources;

namespace SunsetSystems.Entities.Characters
{
    [CreateAssetMenu(fileName = "New Creature Config", menuName = "Character/Creature Config")]
    public class CreatureConfig : ScriptableObject
    {
        [SerializeField]
        private string _name = "New";
        public string Name { get => _name; }
        [SerializeField]
        private string _lastName = "Creature";
        public string LastName { get => _lastName; }
        public string FullName { get => Name + " " + LastName; }
        [SerializeField]
        private Sprite _portrait;
        public Sprite Portrait { get => _portrait; }
        [SerializeField]
        private StatsConfig _statsConfig;
        public StatsConfig StatsAsset { get => _statsConfig; }
        [SerializeField]
        private string _umaPresetFilename = "default";
        public string UmaPresetFilename { get => _umaPresetFilename; }
        [SerializeField]
        private string animatorControllerResourceName = "";
        [SerializeField]
        private RuntimeAnimatorController _animatorController;
        public RuntimeAnimatorController AnimatorController
        {
            get => _animatorController;
        }
        [SerializeField]
        private Faction _creatureFaction;
        public Faction CreatureFaction { get => _creatureFaction; }
        [SerializeField]
        private BodyType _bodyType;
        public BodyType BodyType { get => _bodyType; }
        [SerializeField]
        private CreatureType _creatureType;
        public CreatureType CreatureType { get => _creatureType; }


        private void OnEnable()
        {
            if (_portrait == null)
                _portrait = UnityEngine.Resources.Load<Sprite>("DEBUG/missing");
            if (_statsConfig == null)
                _statsConfig = UnityEngine.Resources.Load<StatsConfig>("DEBUG/DebugStats");
        }

        private void Awake()
        {
            FindAnimatorController();
        }

        private void FindAnimatorController()
        {
            if (_animatorController == null && !animatorControllerResourceName.Equals(""))
                _animatorController = ResourceLoader.GetAnimatorController(animatorControllerResourceName);
            if (_animatorController == null)
                _animatorController = ResourceLoader.GetFallbackAnimator();
        }
    }
}
