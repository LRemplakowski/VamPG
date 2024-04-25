using System;
using UnityEngine;

namespace MyLib.Shared.Database
{
    /// <summary>
    /// Base class used to derive generic databases from.
    /// </summary>
    /// <typeparam name="S">The Database type to use.</typeparam>
    /// <typeparam name="T">The data type the database holds.</typeparam>
    [Serializable]
    public abstract class DatabaseFileBase<S, T> : ScriptableObject, IDatabaseFile
        where S : Database<T>, new()
        where T : DatabaseAsset
    {
        /// <summary>
        /// The database data.
        /// </summary>
        public S data = new S();

        /// <summary>
        /// The Databases ScriptableObject.
        /// </summary>
        public ScriptableObject File { get { return this; } }

        /// <summary>
        /// The databaes 16 bit ID.
        /// </summary>
        public short ID16
        {
            get { return data.ID16; }
        }

        /// <summary>
        /// The databases icon atlas.
        /// </summary>
        public IconAtlas IconAtlas
        {
            get { return data.IconAtlas; }
        }

        /// <summary>
        /// The Icon Atlases texture.
        /// </summary>
        public Texture2D IconTexture
        {
            get { return data.IconTexture; }
            set { data.IconTexture = value; }
        }

        /// <summary>
        /// Database data
        /// </summary>
        public Database DatabaseData { get { return data; } }

        public DatabaseFileBase(string name, short id16)
        {
            data.ID16 = id16;
        }

        /// <summary>
        /// Adds a new database asset.
        /// </summary>
        /// <param name="value">Asset to add.</param>
        /// <returns>The 16 bit ID of the asset.</returns>
        public int AddNew(T value)
        {
            data.AddAsset(value, value.AssetKey16);
            return value.AssetKey16;
        }

        public abstract int AddNew(string name, UnityEngine.Object value);

        /// <summary>
        /// Removes an asset at the given index.
        /// </summary>
        /// <param name="index">Asset index into the database to remove.</param>
        public void RemoveAtIndex(int index)
        {
            data.RemoveAssetByIndex(index);
        }

        /// <summary>
        /// Set a new 16 bit ID for this database.
        /// </summary>
        /// <param name="id16">New 16 bit ID.</param>
        public void SetId16(short id16)
        {
            data.ID16 = id16;
        }

        /// <summary>
        /// Retrieves an asset matching a 16 bit ID.
        /// </summary>
        /// <param name="assetKey16">The 16 bit ID of the asset to retrieve.</param>
        /// <returns>The Asset with the given 16 bit ID.</returns>
        public T GetAsset(short assetKey16)
        {
            return data[assetKey16];
        }

        public short GetAssetID<U>(U asset)
        {
            foreach (var key in data.mKeys)
            {
                var databaseEntry = data[key];
                if (databaseEntry.AssetObject.Equals(asset))
                    return databaseEntry.AssetKey16;
            }
            return -1;
        }

        /// <summary>
        /// The databases visibility, internal or visible
        /// </summary>
        /// <returns>The databases visibility.</returns>
        public Database.Visibility GetVisibility()
        {
            return data.visibility;
        }
    }
}