using System.Collections.Generic;
using Sirenix.OdinInspector;
using SunsetSystems.Entities.Characters.Interfaces;
using UMA;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SunsetSystems.Entities.Characters
{
    public class CreatureData : SerializedMonoBehaviour
    {
        public string FirstName = "Foo", LastName = "Bar";
        public string FullName => $"{FirstName} {LastName}";
        [field: SerializeField, ReadOnly]
        public string ReadableID { get; private set; }
        [field: SerializeField, ReadOnly]
        public string DatabaseID { get; private set; }
        [field: SerializeField]
        public AssetReferenceSprite PortraitAssetRef { get; private set; }
        [field: SerializeField]
        public Faction Faction { get; private set; }
        [field: SerializeField]
        public BodyType BodyType { get; private set; }
        [field: SerializeField]
        public CreatureType CreatureType { get; private set; }

        public void CopyFromTemplate(ICreatureTemplate template)
        {
            FirstName = template.FirstName;
            LastName = template.LastName;
            DatabaseID = template.DatabaseID;
            ReadableID = template.ReadableID;
            PortraitAssetRef = template.PortraitAssetRef;
            Faction = template.Faction;
            BodyType = template.BodyType;
            CreatureType = template.CreatureType;
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode is false)
            {
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}
