using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MyLib.EditorTools.Tools
{
    public class AssetTools
    {
        /// <summary>
        /// Creates and selects a new scriptableObject 
        /// </summary>
        /// <typeparam name="T">The asset type to create.</typeparam>
        /// <returns>a newly created asset or null if canceled.</returns>
        public static ScriptableObject CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New" + typeof(T).ToString() + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            return asset;
        }

        /// <summary>
        /// Opens a scriptableObject creation panel.
        /// </summary>
        /// <typeparam name="T">The scriptableObject type.</typeparam>
        /// <param name="assetName">The name of the asset.</param>
        /// <param name="assetExt">The extension of the asset.</param>
        /// <returns>A newly created asset, or null if canceled.</returns>
        public static T CreateAssetPanel<T>(string assetName, string assetExt) where T : ScriptableObject
        {
            return CreateAssetPanel<T>(assetName, assetExt, out string emptyString);
        }

        /// <summary>
        /// Opens a scriptableObject creation panel.
        /// </summary>
        /// <typeparam name="T">The scriptableObject type.</typeparam>
        /// <param name="assetName">The name of the asset.</param>
        /// <param name="assetExt">The extension of the asset.</param>
        /// <param name="savePath">The save path of the asset.</param>
        /// <returns>A newly created asset, or null if canceled.</returns>
        public static T CreateAssetPanel<T>(string assetName, string assetExt, out string savePath) where T : ScriptableObject
        {
            savePath = EditorUtility.SaveFilePanel("Select a Folder to save the '" + typeof(T) +  "' to.",
            Application.dataPath,
            assetName, assetExt);

            if (!System.String.IsNullOrEmpty(savePath))
            {
                savePath = savePath.Replace(Application.dataPath, "Assets");

                if (!savePath.StartsWith("Assets/"))
                    savePath = "Assets/";

                T asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, savePath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Created Asset '" + savePath + "'.");

                savePath = savePath.Replace(".asset", "");

                return asset;
            }
            else
            {
                Debug.Log("Canceled Asset creation.");
                return null;
            }
        }

        /// <summary>
        /// Opens an asset duplication panel.
        /// </summary>
        /// <param name="sourceObject">The asset to duplicate.</param>
        /// <param name="assetName">The name of the asset.</param>
        /// <param name="assetExt">The extension of the asset.</param>
        /// <param name="savePath">The save location of the asset.</param>
        /// <returns>A copy of the sourceObject.</returns>
        public static Object DuplicateAssetPanel(Object sourceObject, string assetName, string assetExt, out string savePath)
        {
            savePath = EditorUtility.SaveFilePanel("Select a Folder to save the '" + sourceObject.name + "' to.",
            Application.dataPath,
            assetName, assetExt);

            if (!System.String.IsNullOrEmpty(savePath))
            {
                savePath = savePath.Replace(Application.dataPath, "Assets");

                if (!savePath.StartsWith("Assets/"))
                    savePath = "Assets/";

                Object asset = ScriptableObject.Instantiate(sourceObject);

                AssetDatabase.CreateAsset(asset, savePath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Created Asset '" + savePath + "'.");

                savePath = savePath.Replace(".asset", "");

                return asset;
            }
            else
            {
                Debug.Log("Canceled Asset creation.");
                return null;
            }
        }

        /// <summary>
        /// Get a list of dragged assets by type. Uses GUILayoutUtility.GetLastRect()
        /// </summary>
        /// <typeparam name="t">The asset Type to check against.</typeparam>
        /// <returns>A UnityEngine.Object[] list of the type specified, or null if none are present.</returns>
        public static T[] CheckAssetDrag<T>()
            where T : Object
        {
            List<T> returns = new List<T>();

            // Get The screen position of last used GUILayout Element
            Rect lastRect = GUILayoutUtility.GetLastRect();

            // Handle events
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                    // Test against rect from last repaint
                    if (lastRect.Contains(evt.mousePosition))
                    {
                        // Change cursor and consume the event
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        evt.Use();
                    }
                    break;

                case EventType.DragPerform:
                    // Test against rect from last repaint
                    if (lastRect.Contains(evt.mousePosition))
                    {
                        // Handle the drop however you want to
                        foreach (Object obj in DragAndDrop.objectReferences)
                        {
                            if (obj.GetType() == typeof(T) || obj.GetType().IsSubclassOf(typeof(T)))
                                returns.Add((T)obj);
                        }

                        // Change cursor and consume the event and drag
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        DragAndDrop.AcceptDrag();
                        evt.Use();
                    }
                    break;
            }
            return returns.Count > 0 ? returns.ToArray() : null;
        }

        /// <summary>
        /// Used to get assets of a certain type and File extension from entire project
        /// </summary>
        /// <typeparam name="t">The asset type to check against. Example: "GameObject".</typeparam>
        /// <param name="fileExtension">File extension the type uses Example: ".prefab".</param>
        /// <returns>An Object array of assets, or null if none where found.</returns>
        public static T[] GetAssetsOfType<T>(string fileExtension)
            where T : Object
        {
            List<T> tempObjects = new List<T>();
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            FileInfo[] goFileInfo = directory.GetFiles("*." + fileExtension, SearchOption.AllDirectories);

            int i = 0, goFileInfoLength = goFileInfo.Length;
            FileInfo tempGoFileInfo; string tempFilePath;
            T tempObj;
            for (; i < goFileInfoLength; i++)
            {
                tempGoFileInfo = goFileInfo[i];
                if (tempGoFileInfo == null)
                    continue;

                tempFilePath = tempGoFileInfo.FullName;
                tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");

                tempObj = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(T)) as T;
                if (tempObj == null)
                    continue;
                else if (tempObj.GetType() != typeof(T))
                    continue;

                tempObjects.Add(tempObj);
            }

            return tempObjects.Count > 0 ? tempObjects.ToArray() : new T[0];
        }

        /// <summary>
        /// Finds all assets in the project with a given extension.
        /// </summary>
        /// <param name="fileExtension">The extension to search for.</param>
        /// <returns>A list of Objects that are of the specified extension.</returns>
        public static Object[] GetAssetsWithExtension(string fileExtension)
        {
            List<Object> tempObjects = new List<Object>();
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            FileInfo[] goFileInfo = directory.GetFiles("*." + fileExtension, SearchOption.AllDirectories);

            int i = 0, goFileInfoLength = goFileInfo.Length;
            FileInfo tempGoFileInfo; string tempFilePath;
            Object tempObj;
            for (; i < goFileInfoLength; i++)
            {
                tempGoFileInfo = goFileInfo[i];
                if (tempGoFileInfo == null)
                    continue;

                tempFilePath = tempGoFileInfo.FullName;
                tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");

                tempObj = AssetDatabase.LoadAssetAtPath(tempFilePath, typeof(Object));
                if (tempObj == null)
                    continue;

                tempObjects.Add(tempObj);
            }

            return tempObjects.Count > 0 ? tempObjects.ToArray() : new Object[0];
        }

        /// <summary>
        /// Creates a readable texture. Even from internal unity textures.
        /// </summary>
        /// <param name="icon">The texture to Create from.</param>
        /// <param name="renderWidth">The width of the icon.</param>
        /// <param name="renderHeight">The height of the icon.</param>
        /// <returns></returns>
        public static Texture2D CreateReadableTexture(Texture2D icon, int renderWidth, int renderHeight)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                                        renderWidth,
                                        renderHeight,
                                        0,
                                        RenderTextureFormat.Default,
                                        RenderTextureReadWrite.Linear);

            Graphics.Blit(icon, tmp);

            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(renderWidth, renderHeight);
            myTexture2D.ReadPixels(new Rect(0, 0, renderWidth, renderHeight), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(tmp);
            return myTexture2D;
        }
    }
}