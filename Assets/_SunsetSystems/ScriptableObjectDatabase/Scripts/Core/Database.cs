using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace MyLib.Shared.Database
{
    /// <summary>
    /// Interface used with Scriptable Object to define a database file.
    /// </summary>
    public interface IDatabaseFile
    {
        ScriptableObject File { get; }

        /// <summary>
        /// The icons that represent the assets.
        /// </summary>
        IconAtlas IconAtlas { get; }

        /// <summary>
        /// The Icon Atlas Texture.
        /// </summary>
        Texture2D IconTexture { get; set; }

        /// <summary>
        /// Holds the asset data of the database.
        /// </summary>
        Database DatabaseData { get; }

        /// <summary>
        /// The 16 bit database ID.
        /// </summary>
        short ID16 { get; }

        /// <summary>
        /// Add new Asset to the database.
        /// </summary>
        /// <param name="name">Asset Name</param>
        /// <param name="value">Unity Object to add.</param>
        /// <returns></returns>
        int AddNew(string name, UnityObject value);

        /// <summary>
        /// Set a new database ID.
        /// </summary>
        /// <param name="databaseKey16">16 bit ID of the database.</param>
        void SetId16(short databaseKey16);

        /// <summary>
        /// Removes an asset from the database.
        /// </summary>
        /// <param name="index"></param>
        void RemoveAtIndex(int index);

        /// <summary>
        /// Visibility of the database, 'Internal' or 'Visible'.
        /// </summary>
        /// <returns></returns>
        Database.Visibility GetVisibility();
    }

    /// <summary>
    /// Base database data.
    /// </summary>
    [System.Serializable]
    public abstract class Database
    {
        /// <summary>
        /// Can be used to limit a databases visibilty to the user.
        /// </summary>
        public enum Visibility
        {
            Visible = 0,
            Internal = 1,
        }

        /// <summary>
        /// 16 bit id of the database.
        /// </summary>
        public int id;

        /// <summary>
        /// 16 bit asset key list.
        /// </summary>
        public List<int> mKeys = new List<int>();

        /// <summary>
        /// Database visiblity.
        /// </summary>
        public Visibility visibility = Visibility.Visible;

        /// <summary>
        /// Holds the assets icons.
        /// </summary>
        [SerializeField]
        protected IconAtlas pIcons = new IconAtlas();

        /// <summary>
        /// 16 bit database id.
        /// </summary>
        public short ID16 { get { return (short)id; } set { id = value; } }

        /// <summary>
        /// Holds Asset Icons.
        /// </summary>
        public IconAtlas IconAtlas { get { return pIcons; } set { pIcons = value; } }

        /// <summary>
        /// The Icon Atlas texture.
        /// </summary>
        public Texture2D IconTexture { get { return pIcons.Texture; } set { pIcons.Texture = value; } }

        /// <summary>
        /// Does the Database contain asset data.
        /// </summary>
        public bool IsEmpty { get { return mKeys.Count <= 0; } }

        /// <summary>
        /// Num of assets the database references.
        /// </summary>
        public int NumOfEntries { get { return mKeys.Count; } }

        /// <summary>
        /// Retrieves a unique asset key for this database.
        /// </summary>
        public short UniqueAssetKey
        {
            get
            {
                short key16 = (short)UnityEngine.Random.Range(0, short.MaxValue);

                while (mKeys.Contains(key16))
                    key16 = (short)UnityEngine.Random.Range(0, short.MaxValue);

                return key16;
            }
        }

        /// <summary>
        /// Gets the Key at a certain index in the asset key list.
        /// </summary>
        /// <param name="index">Index into the list.</param>
        /// <returns>The key at the specified index.</returns>
        public short KeyAtIndex(int index)
        {
            if (index < 0)
                return -1;

            return (short)mKeys[index];
        }

        /// <summary>
        /// Does the database contain a certain 16 bit asset ID.
        /// </summary>
        /// <param name="key16">16 bit asset ID</param>
        /// <returns>True if database contains asset ID, false if database is missing the key..</returns>
        public bool ContainsKey(short key16)
        {
            for (int i = 0; i < mKeys.Count; i++)
            {
                if (mKeys[i] == key16)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the index of a certain DatabaseAsset
        /// </summary>
        /// <param name="curAsset">The asset to use in retrieving the Key Index.</param>
        /// <returns>The index into the key array of the specified asset.</returns>
        public int IndexOf(DatabaseAsset curAsset)
        {
            return mKeys.IndexOf(curAsset.AssetKey16);
        }

        /// <summary>
        /// Moves an asset up or down in the asset list.
        /// </summary>
        /// <param name="fromIndex">The current asset index.</param>
        /// <param name="up">Shift up or down</param>
        public void Shift(int fromIndex, bool up)
        {
            int toIndex = fromIndex + (up ? -1 : 1);

            if ((fromIndex < 0) || (fromIndex >= mKeys.Count))
                return;
            else if ((toIndex < 0) || (toIndex >= mKeys.Count))
                return;

            ShiftFromTo(fromIndex, toIndex);
        }

        /// <summary>
        /// Checks to see if database contains a certain instance id.
        /// </summary>
        /// <param name="instanceId">Id to check</param>
        /// <returns>True if database contains the instance ID.</returns>
        abstract public bool ContainsInstanceId(int instanceId);

        /// <summary>
        /// Gets an asset at a certain index.
        /// </summary>
        /// <param name="index">Index into asset list to look.</param>
        /// <returns>Asset at the index.</returns>
        abstract public DatabaseAsset GetAssetAtIndex(int index);

        /// <summary>
        /// Get an asset with a certain 16 bit ID.
        /// </summary>
        /// <param name="assetKey16">16 bit asset ID.</param>
        /// <returns>The asset matching the specified ID.</returns>
        abstract public DatabaseAsset GetAsset(short assetKey16);

        /// <summary>
        /// Removes a certain asset from the database.
        /// </summary>
        /// <param name="assetValue"></param>
        abstract public void RemoveAssetByValue(DatabaseAsset assetValue);

        /// <summary>
        /// Moves an asset from one position in the list to another.
        /// </summary>
        /// <param name="fromIndex">Current index position.</param>
        /// <param name="toIndex">Destination index position.</param>
        abstract protected void ShiftFromTo(int fromIndex, int toIndex);
    }

    [Serializable]
    public class Database<T> : Database
        where T : DatabaseAsset
    {
        #region Variables

        [SerializeField]
        private List<T> mVals = new List<T>();

        #endregion Variables

        /// <summary>
        /// The data held in this database.
        /// </summary>
        public List<T> Data { get { return mVals; } }

        /// <summary>
        /// Accessor for data using 16 bit asset ID.
        /// </summary>
        /// <param name="key16">16 bit asset ID.</param>
        /// <returns>The data that matching the specified asset ID.</returns>
        public T this[int key16]
        {
            get
            {
                int index = mKeys.IndexOf(key16);

                if (index == -1)
                    return default(T);

                return mVals[index];
            }
        }

        /// <summary>
        /// Does the database contain a certain value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if the database contains the specified value, false if not.</returns>
        public bool ContainsValue(T value)
        {
            for (int i = 0; i < mVals.Count; i++)
            {
                if (mVals[i].AssetObject.GetInstanceID() == value.AssetObject.GetInstanceID())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Add an asset to the database.
        /// </summary>
        /// <param name="value">Asset to add.</param>
        /// <param name="key16">16 bit Id to assign to the asset.</param>
        /// <returns></returns>
        public int AddAsset(T value, short key16)
        {
            mKeys.Add(key16);
            mVals.Add(value);
            return key16;
        }

        public override void RemoveAssetByValue(DatabaseAsset assetValue)
        {
            mVals.IndexOf((T)assetValue);

            for (int i = 0; i < mVals.Count; i++)
            {
                if (mVals[i].Equals(assetValue))
                {
                    mVals.RemoveAt(i);
                    mKeys.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes a certain asset from the database at a certain index.
        /// </summary>
        /// <param name="index">Index of the asset to remove.</param>
        public void RemoveAssetByIndex(int index)
        {
            mVals.RemoveAt(index);
            mKeys.RemoveAt(index);
        }

        /// <summary>
        /// Moves an asset from one position in the list to another.
        /// </summary>
        /// <param name="fromIndex">Current index position.</param>
        /// <param name="toIndex">Destination index position.</param>
        override protected void ShiftFromTo(int fromIndex, int toIndex)
        {
            int fromKey, toKey;
            T fromData, toData;

            fromKey = mKeys[fromIndex];
            fromData = mVals[fromIndex];
            toKey = mKeys[toIndex];
            toData = mVals[toIndex];

            mKeys[fromIndex] = toKey;
            mVals[fromIndex] = toData;

            mKeys[toIndex] = fromKey;
            mVals[toIndex] = fromData;
        }

        /// <summary>
        /// Replaces asset value using a 16 bit ID.
        /// </summary>
        /// <param name="assetKey16">16 bit asset ID.</param>
        /// <param name="value">Asset to set.</param>
        public void SetAsset(short assetKey16, DatabaseAsset value)
        {
            int index = mKeys.IndexOf(assetKey16);
            mVals[index] = (T)value;
        }

        /// <summary>
        /// Gets an asset at a certain index.
        /// </summary>
        /// <param name="index">Index into asset list to look.</param>
        /// <returns>Asset at the index.</returns>
        public override DatabaseAsset GetAssetAtIndex(int index)
        {
            return Data[index];
        }

        /// <summary>
        /// Get an asset with a certain 16 bit ID.
        /// </summary>
        /// <param name="assetKey16">16 bit asset ID.</param>
        /// <returns>The asset matching the specified ID.</returns>
        public override DatabaseAsset GetAsset(short assetKey16)
        {
            return this[assetKey16];
        }

        /// <summary>
        /// Checks to see if database contains a certain instance id.
        /// </summary>
        /// <param name="instanceId">Id to check</param>
        /// <returns>True if database contains the instance ID.</returns>
        public override bool ContainsInstanceId(int instanceId)
        {
            for (int i = 0; i < mVals.Count; i++)
            {
                if (mVals[i].AssetObject.GetInstanceID() == instanceId)
                    return true;
            }
            return false;
        }
    }

    public abstract class DatabaseAsset
    {
        [SerializeField]
        protected string pName = "Data Name";

        [SerializeField]
        protected AssetKey32 pKey;

        /// <summary>
        /// The name of the database asset.
        /// </summary>
        public string Name { get { return pName; } set { pName = value; } }

        /// <summary>
        /// The 16 bit database ID.
        /// </summary>
        public short DatabaseID16 { get { return pKey.DatabaseKey; } }

        /// <summary>
        /// The 16 bit asset id.
        /// </summary>
        public short AssetKey16 { get { return (short)pKey.AssetKey; } }

        /// <summary>
        /// The combined database and asset id wrapped into a 32 bit integer.
        /// </summary>
        public int ID32 { get { return pKey.Key32; } }

        /// <summary>
        /// Does the database have errors that need attention?
        /// </summary>
        public virtual bool HasErrors { get { return false; } }

        public DatabaseAsset(string name, short databaseId16, short assetKey16)
        {
            pName = name;
            pKey = new AssetKey32(databaseId16, assetKey16);
        }

        public bool Equals(DatabaseAsset other)
        {
            if (other == null)
                return false;

            if (AssetKey16.Equals(other.AssetKey16) && pName.Equals(other.Name))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns the UnityObject the asset references.
        /// </summary>
        public abstract UnityObject AssetObject { get; }

        /// <summary>
        /// Sets a new UnityObject to the asset entry.
        /// </summary>
        /// <param name="newObject">The UnityObject reference to set this asset to.</param>
        public abstract void SetObject(UnityObject newObject);
    }

    [Serializable]
    public class DatabaseAsset<T> : DatabaseAsset 
        where T : UnityObject
    {
        #region Variables

        [SerializeField]
        private T mAsset;

        /// <summary>
        /// The Explicity asset this database entry references.
        /// </summary>
        public T Asset { get { return mAsset; } set { mAsset = value; } }

        /// <summary>
        /// The UnityObject version of the asset this database entry references.
        /// </summary>
        public override UnityObject AssetObject { get { return mAsset; } }

        #endregion Variables

        public DatabaseAsset(string name, short databaseId16, short assetKey16, T asset)
            : base(name, databaseId16, assetKey16)
        {
            this.mAsset = asset;
        }

        /// <summary>
        /// Set a new asset reference for this database entry.
        /// </summary>
        /// <param name="newObject">Asset object to set.</param>
        public override void SetObject(UnityObject newObject)
        {
            if (newObject.GetType() == typeof(T) || newObject.GetType().IsSubclassOf(typeof(T)))
                mAsset = newObject as T;
            else
                Debug.Log("Invalid Object type.");
        }
    }

    [Serializable]
    public class IconAtlas
    {
        [SerializeField]
        private List<int> mKeys = new List<int>();

        [SerializeField]
        private List<Rect> mUvs = new List<Rect>();

        [SerializeField]
        private Texture2D mTexture;

        /// <summary>
        /// Texture used to save the icons to, and load them from.
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                return mTexture;
            }
            set
            {
                mTexture = value;
                mTexture.Apply();
            }
        }

        /// <summary>
        /// Get the Rect region of the texture that contains the input ID.
        /// </summary>
        /// <param name="key16">The ID of the icon rect to retrieve.</param>
        /// <returns></returns>
        public Rect this[int key16]
        {
            get
            {
                int index = mKeys.IndexOf(key16);
                if (index == -1)
                {
                    Debug.LogError("uvs not found for " + key16);
                    return new Rect(0, 0, 1, 1);
                }
                return mUvs[index];
            }
        }

        /// <summary>
        /// Number of icons.
        /// </summary>
        public int Count { get { return mUvs.Count; } }

        /// <summary>
        /// Does the icon atlas contain a certain icon.
        /// </summary>
        /// <param name="id16">Icon ID</param>
        /// <returns>True if atlas has an icon matching the input ID.</returns>
        public bool Contains(int id16)
        {
            return mKeys.Contains(id16);
        }

        /// <summary>
        /// Gets the ID at a certain index into the icon list.
        /// </summary>
        /// <param name="index">The index into the icon list.</param>
        /// <returns>ID of the icon at the index into the icon list.</returns>
        public int GetKeyAtIndex(int index)
        {
            return mKeys[index];
        }

        /// <summary>
        /// Set an icon rect in the icon atlas.
        /// </summary>
        /// <param name="index">Index into the icon list.</param>
        /// <param name="value">New Rect to set as the icons uvs.</param>
        /// <returns></returns>
        public Rect SetUvsAtIndex(int index, Rect value)
        {
            return mUvs[index] = value;
        }

        /// <summary>
        /// Adds a new icon.
        /// </summary>
        /// <param name="key16">16 bit ID.</param>
        /// <param name="uvs">The Rect, or uvs of the icon.</param>
        public void Add(int key16, Rect uvs)
        {
            mKeys.Add(key16);
            mUvs.Add(uvs);
        }

        /// <summary>
        /// Removes a certain icon from the list.
        /// </summary>
        /// <param name="key16">ID of icon to remove.</param>
        public void Remove(int key16)
        {
            int index = mKeys.IndexOf(key16);

            if (index != -1)
            {
                mKeys.RemoveAt(index);
                mUvs.RemoveAt(index);
            }
            else
                Debug.Log("Could not find icon for removal. Key: " + key16);
        }

        /// <summary>
        /// Gets the pixel coords of a certain icon.
        /// </summary>
        /// <param name="key16">16 bit icon ID.</param>
        /// <returns>Rect or pixel uvs for the specified icon.</returns>
        public Rect PixelCoords(int key16)
        {
            int index = mKeys.IndexOf(key16);

            return new Rect(mUvs[index].x * mTexture.width,
                            mUvs[index].y * mTexture.height,
                            mUvs[index].width * mTexture.width,
                            mUvs[index].height * mTexture.height);
        }
    }
}