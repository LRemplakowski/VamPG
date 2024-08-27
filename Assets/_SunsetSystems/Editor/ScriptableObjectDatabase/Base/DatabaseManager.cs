using System.Collections.Generic;
using MyLib.Shared.Database;
using MyLib.EditorTools.Tools;
using UnityEditor;
using UnityEngine;

namespace MyLib.EditorTools
{
    /// <summary>
    /// Maintains the main database list, and contains methods for editors to retrieve data from database.
    /// </summary>
    public class DatabaseManager
    {
        /// <summary>
        /// A post processor keeps track of new and deleted databases.
        /// </summary>
        private class DatabasePostProcessor : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(
                string[] importedAssets,
                string[] deletedAssets,
                string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                Object curObject;
                foreach (string str in importedAssets)
                {
                    curObject = AssetDatabase.LoadAssetAtPath(str, typeof(Object));

                    if (curObject is IDatabaseFile)
                    {
                        ImportDatabase(curObject as IDatabaseFile);
                        mLoaded = true;
                    }
                }

                foreach (string str in deletedAssets)
                {
                    curObject = AssetDatabase.LoadAssetAtPath(str, typeof(Object));

                    //Debug.Log(AssetDatabase.AssetPathToGUID(str));

                    if (curObject is IDatabaseFile)
                    {
                        mDatabaseCollection.RemoveDatabase(curObject as IDatabaseFile);
                        mLoaded = false;
                    }
                }

                if (!mLoaded)
                    LoadDatabaseList();
            }
        }

        private static bool mLoaded = false;
        private static MainDatabaseCollection mDatabaseCollection;

        public static void SetDirty(IDatabaseFile file)
        {
            
            if (!mLoaded) LoadDatabaseList();

            EditorUtility.SetDirty(file.File);

            ImportDatabase(file);
        }

        /// <summary>
        /// Loads all databases.
        /// </summary>
        public static void LoadDatabaseList()
        {
            mLoaded = true;

            if (mDatabaseCollection == null)
            {
                MainDatabaseCollection[] collections = AssetTools.GetAssetsOfType<MainDatabaseCollection>("asset");

                if (collections.Length == 0)
                {
                    EditorUtility.DisplayDialog("Create Main Database File", "Please select a location to save the Main Database Collection File to." +
                        "\n This file is used to store and access all Databases that are located in the project.", "Next");

                    mDatabaseCollection = AssetTools.CreateAssetPanel<MainDatabaseCollection>("DatabaseCollection", "asset");
                    if (mDatabaseCollection == null) //Cancled
                    {
                        Debug.Log("Canceled Main Database Collection Creation.");
                        mLoaded = false;
                        return;
                    }
                }
                else
                {
                    mDatabaseCollection = collections[0];

                    if (collections.Length > 1)
                        Debug.Log("Multiple Database Lists no allowed, using first one found.");
                }
            }

            mDatabaseCollection.visibleFiles.Clear();
            mDatabaseCollection.hiddenFiles.Clear();

            Object[] list = AssetTools.GetAssetsWithExtension("asset");
            foreach (Object obj in list)
            {
                if (obj is IDatabaseFile)
                {
                    ImportDatabase(obj as IDatabaseFile);
                }
            }

            EditorUtility.SetDirty(mDatabaseCollection);
        }

        /// <summary>
        /// Adds a database to the main database collection
        /// </summary>
        /// <param name="file"></param>
        public static void ImportDatabase(IDatabaseFile file)
        {
            if (!mLoaded) LoadDatabaseList();

            mDatabaseCollection.ImportDatabase(file);
        }

        /// <summary>
        /// Scans the database list and creates a new unique database ID.
        /// </summary>
        /// <returns>A unique Database ID.</returns>
        public static short GetUniqueDatabaseId()
        {
            if (!mLoaded) LoadDatabaseList();

            return mDatabaseCollection.GetUniqueDatabaseId();
        }

        /// <summary>
        /// Gets all databases of a certain type.
        /// </summary>
        /// <typeparam name="T">The database type to retrieve.</typeparam>
        /// <param name="visibility">The visibility of the databases.</param>
        /// <returns></returns>
        public static T[] GetDatabasesOfType<T>(Database.Visibility visibility)
            where T : IDatabaseFile
        {
            if (!mLoaded) LoadDatabaseList();

            return mDatabaseCollection.GetDatabasesOfType<T>(visibility);
        }

        /// <summary>
        /// Gets a database of a certain 16 bit ID.
        /// </summary>
        /// <param name="id16">The 16 bit ID of the database.</param>
        /// <returns></returns>
        public static IDatabaseFile GetDatabase(short id16)
        {
            if (!mLoaded) LoadDatabaseList();
            return mDatabaseCollection.GetDatabase(id16);
        }

        /// <summary>
        /// Uses the unique 32 bit ID to get an asset.
        /// </summary>
        /// <param name="key32">32 bit key contains both 16 bit database and asset IDs.</param>
        /// <returns></returns>
        public static DatabaseAsset GetAsset(int key32)
        {
            if (!mLoaded) LoadDatabaseList();
            return mDatabaseCollection.GetAsset(key32);
        }
    }
}