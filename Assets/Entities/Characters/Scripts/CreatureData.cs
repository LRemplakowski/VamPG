using UnityEngine;

namespace Entities.Characters
{
    public class CreatureData : ExposableMonobehaviour
    {
        private CreatureAsset _dataInstance;
        [SerializeField]
        private CreatureAsset data;
        [SerializeField, ReadOnly]
        private string _firstName, _lastName;
        public string FirstName 
        { 
            get => _dataInstance.CreatureName; 
            set
            {
                _dataInstance.CreatureName = value;
                _firstName = value;
            }
        }
        public string LastName 
        { 
            get => _dataInstance.CreatureLastName;
            set
            {
                _dataInstance.CreatureLastName = value;
                _lastName = value;
            }
        }
        public string FullName { get => FirstName + " " + LastName; }
        [SerializeField, ReadOnly]
        private Sprite _portrait;
        public Sprite Portrait 
        { 
            get => _portrait; 
            set
            {
                _dataInstance.Portrait = value;
                _portrait = value;
            }
        }
        [SerializeField, ReadOnly]
        private Faction _faction;
        public Faction Faction 
        { 
            get => _faction; 
            set
            {
                _dataInstance.CreatureFaction = value;
                _faction = value;
            }
        }
        [SerializeField, ReadOnly]
        private BodyType _bodyType = BodyType.F;
        public BodyType BodyType 
        { 
            get => _bodyType; 
            set
            {
                _dataInstance.BodyType = value;
                _bodyType = value;
            }
        }
        [SerializeField, ReadOnly]
        private CreatureType _creatureType;
        public CreatureType CreatureType 
        { 
            get => _creatureType; 
            set
            {
                _dataInstance.CreatureType = value;
                _creatureType = value;
            }
        }

        private void Reset()
        {
            if (data == null)
                data = Resources.Load<CreatureAsset>("DEBUG/default");
            if (_dataInstance == null)
                _dataInstance = data;
            gameObject.name = data.name;
            InitializeData();
        }

        private void Start()
        {
            _dataInstance = CreatureAsset.CopyInstance(data);
            InitializeData();
        }

        private void InitializeData()
        {
            if (_dataInstance)
            {
                CreatureInitializer.InitializeCreature(gameObject, _dataInstance, gameObject.transform.position);
                _portrait = _dataInstance.Portrait;
                _faction = _dataInstance.CreatureFaction;
                _firstName = _dataInstance.CreatureName;
                _lastName = _dataInstance.CreatureLastName;
                _bodyType = _dataInstance.BodyType;
                _creatureType = _dataInstance.CreatureType;
            }
        }

        [ContextMenu("Initialize Creature from Data")]
        private void InitializeInEditor()
        {
            _dataInstance = data;
            gameObject.name = _dataInstance.name;
            InitializeData();
        }
    }
}
