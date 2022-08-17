using UnityEngine;

namespace Entities.Characters
{
    public class CreatureData : ExposableMonobehaviour
    {
        [SerializeField]
        private CreatureAsset _dataInstance;
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
        public Creature CreatureComponent
        {
            get;
            private set;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (_dataInstance == null)
                _dataInstance = Resources.Load<CreatureAsset>("DEBUG/default");
            gameObject.name = _dataInstance.name;
            InitializeData();
        }
#endif

        private void Start()
        {
            CreateCreature();
        }

        public void CreateCreature()
        {
            if (_dataInstance)
            {
                InitializeData();
            }
        }

        public void SetData(CreatureAsset data)
        {
            this._dataInstance = data;
        }

        private void InitializeData()
        {
            if (_dataInstance)
            {
                gameObject.name = _dataInstance.CreatureName + " " + _dataInstance.CreatureLastName;
                CreatureComponent = CreatureInitializer.InitializeCreature(gameObject, _dataInstance, gameObject.transform.position);
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
            gameObject.name = _dataInstance.name;
            InitializeData();
        }
    }
}
