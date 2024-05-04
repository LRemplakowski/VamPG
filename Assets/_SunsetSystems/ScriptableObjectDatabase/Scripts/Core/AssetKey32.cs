using System;
using UnityEngine;

namespace MyLib.Shared.Database
{
    /// <summary>
    /// Holds two 16bit inventory IDs in a single 32bit integer. 
    /// One ID for the database, and one ID for the asset.
    /// </summary>
    [Serializable]
    public struct AssetKey32
    {
        [SerializeField]
        private int mKey32;

        /// <summary>
        /// Both parts of the key.
        /// </summary>
        public int Key32
        {
            get { return mKey32; }
        }

        /// <summary>
        /// The first half of the 32 bit key, representing the database ID.
        /// </summary>
        public short DatabaseKey
        {
            get { return (short)(mKey32 >> 16); }
        }

        /// <summary>
        /// The second half of the 32 bit key, representing the Asset ID.
        /// </summary>
        public short AssetKey
        {
            get { return (short)(mKey32 & 0xffff); }
        }

        public AssetKey32(short databaseId16, short assetId16)
        {
            mKey32 = (databaseId16 << 16) | (assetId16 & 0xffff);
        }

        public AssetKey32(int key32)
        {
            this.mKey32 = key32;
        }

        /// <summary>
        /// Processes a 32 bit input and returns the last 16 bits.
        /// </summary>
        /// <param name="i32">32 bit input</param>
        /// <returns>16 bit value representing right half of 32 bit value.</returns>
        public static short Get16BitValueRight(int i32)
        {
            return (short)(i32 & 0xffff);
        }

        /// <summary>
        /// Processes a 32 bit input and returns the first 16 bits.
        /// </summary>
        /// <param name="i32">32 bit input</param>
        /// <returns>16 bit value representing left half of 32 bit value.</returns>
        public static short Get16BitValueLeft(int i32)
        {
            return (short)(i32 & 0xffff);
        }

        /// <summary>
        /// Allows setting new keys after creation.
        /// </summary>
        /// <param name="databaseId16">16 bit database id.</param>
        /// <param name="assetId16">16 bit asset id.</param>
        public void SetKey(short databaseId16, short assetId16)
        {
            mKey32 = (databaseId16 << 16) | (assetId16 & 0xffff);
        }
    }
}