using Sirenix.OdinInspector;
using SunsetSystems.Core.Database;
using UMA.CharacterSystem;
using UnityEditor;
using UnityEngine;

namespace SunsetSystems.UMA
{
    [CreateAssetMenu(fileName = "Wardrobe Data", menuName = "UMA/Base Look Wardrobe")]
    public class WardrobeDatabaseItem : AbstractDatabaseEntry<IUMAWardrobeDatabaseItem>, IUMAWardrobeDatabaseItem
    {
        [field: SerializeField, ReadOnly]
        public override string DatabaseID { get; protected set; }
        [field: SerializeField]
        public override string ReadableID { get; protected set; }

        [field: SerializeField]
        public UMAWardrobeCollection Data { get; private set; }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (Data == null)
            {
                Data = CreateInstance<UMAWardrobeCollection>();
                Data.name = $"{name} Wardrobe Data";
            }
            if (AssetDatabase.IsSubAsset(Data) is false)
            {
                if (AssetDatabase.IsMainAsset(Data))
                {
                    var tempData = Instantiate(Data);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(Data));
                    Data = tempData;
                    Data.name = $"{name} Wardrobe Data";
                }
                AssetDatabase.AddObjectToAsset(Data, this);  //Database is also a ScriptableObject asset.
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
#endif

        protected override void RegisterToDatabase()
        {
            UMAWardrobeDatabase.Instance.Register(this);
        }

        protected override void UnregisterFromDatabase()
        {
            UMAWardrobeDatabase.Instance.Unregister(this);
        }
    }
}
