using MyLib.Shared.Database;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Scriptable object used to keep track of all the databases in the project.
/// </summary>
[System.Serializable]
public class MainDatabaseCollection : ScriptableObject
{
    public List<ScriptableObject> visibleFiles = new List<ScriptableObject>();
    public List<ScriptableObject> hiddenFiles = new List<ScriptableObject>();

    /// <summary>
    /// Retrieves a specific database.
    /// </summary>
    /// <param name="id16">Database 16 bit ID.</param>
    /// <returns>Database or null if database does not exist.</returns>
    public IDatabaseFile GetDatabase(short id16)
    {
        foreach (IDatabaseFile db in visibleFiles)
            if (db.ID16 == id16)
                return db;

        foreach (IDatabaseFile db in hiddenFiles)
            if (db.ID16 == id16)
                return db;

        Debug.LogWarning("Database : " + id16 + ", was not found.");
        return null;
    }


    /// <summary>
    /// Gets an asset using the 32 bit ID
    /// </summary>
    /// <param name="key32">32 bit ID containing both database and asset IDs.</param>
    /// <returns>The specified asset or null if none was found.</returns>
    public DatabaseAsset GetAsset(int key32)
    {
        AssetKey32 key = new AssetKey32(key32);

        foreach (IDatabaseFile database in visibleFiles)
        {
            if (database.ID16 == key.DatabaseKey)
            {
                return database.DatabaseData.GetAsset(key.AssetKey);
            }
        }

        foreach (IDatabaseFile database in hiddenFiles)
        {
            if (database.ID16 == key.DatabaseKey)
            {
                return database.DatabaseData.GetAsset(key.AssetKey);
            }
        }

        Debug.LogWarning("Asset : " + key32 + ", was not found.");
        return null;
    }

    /// <summary>
    /// Removes a database from the List.
    /// </summary>
    /// <param name="file">The database to remove.</param>
    public void RemoveDatabase(IDatabaseFile file)
    {
        if (hiddenFiles.Contains(file.File))
            hiddenFiles.Remove(file.File);

        if (visibleFiles.Contains(file.File))
            visibleFiles.Remove(file.File);
    }

    /// <summary>
    /// Inports a new database to the list.
    /// </summary>
    /// <param name="file">The database to import.</param>
    public void ImportDatabase(IDatabaseFile file)
    {
        if (file.GetVisibility() == Database.Visibility.Visible)
        {
            if (hiddenFiles.Contains(file.File))
                hiddenFiles.Remove(file.File);

            if (!visibleFiles.Contains(file.File))
                visibleFiles.Add(file.File);
        }
        else if (file.GetVisibility() == Database.Visibility.Internal)
        {
            if (visibleFiles.Contains(file.File))
                visibleFiles.Remove(file.File);

            if (!hiddenFiles.Contains(file.File))
                hiddenFiles.Add(file.File);
        }
    }

    /// <summary>
    /// Gets a unique database ID.
    /// </summary>
    /// <returns>Unique database ID.</returns>
    public short GetUniqueDatabaseId()
    {
        List<short> usedKeys = new List<short>();
        for (int i = 0; i < visibleFiles.Count; i++)
            usedKeys.Add(((IDatabaseFile)visibleFiles[i]).ID16);

        for (int i = 0; i < hiddenFiles.Count; i++)
            usedKeys.Add(((IDatabaseFile)hiddenFiles[i]).ID16);

        short key16 = (short)UnityEngine.Random.Range(5, short.MaxValue);

        while (usedKeys.Contains(key16))
            key16 = (short)UnityEngine.Random.Range(5, short.MaxValue);

        return key16;
    }

    /// <summary>
    /// Gets all databases of a certain type.
    /// </summary>
    /// <typeparam name="T">The database type to retrieve.</typeparam>
    /// <param name="visibility">Return databasese using a certain visibility specification.</param>
    /// <returns></returns>
    public T[] GetDatabasesOfType<T>(Database.Visibility visibility)
        where T : IDatabaseFile
    {
        List<T> returns = new List<T>();

        switch (visibility)
        {
            case Database.Visibility.Visible:
                foreach (IDatabaseFile file in visibleFiles)
                {
                    if (file.GetType() == typeof(T))
                        returns.Add((T)file);
                }
                break;

            case Database.Visibility.Internal:
                foreach (IDatabaseFile file in hiddenFiles)
                {
                    if (file.GetType() == typeof(T))
                        returns.Add((T)file);
                }
                break;
        }

        return returns.ToArray();
    }
}