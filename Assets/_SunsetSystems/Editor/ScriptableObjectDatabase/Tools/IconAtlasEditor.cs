using System.Collections.Generic;
using System.IO;
using MyLib.Shared.Database;
using UnityEditor;
using UnityEngine;

namespace MyLib.EditorTools.Tools
{
    public class IconAtlasEditor
    {
        /// <summary>
        /// Adds and icon to the atlas.
        /// </summary>
        /// <param name="icon">Icon texture to add.</param>
        /// <param name="assetKey16">16 bit Asset ID.</param>
        /// <param name="atlas">Atlas to add the icon to.</param>
        /// <param name="parent">The object the atlas is linked to.</param>
        public static void AddIconToAtlas(Texture2D icon, int assetKey16, IconAtlas atlas, ScriptableObject parent)
        {
            AddIconsToAtlas(new Texture2D[] { icon }, new int[] { assetKey16 }, atlas, parent);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="icons">Icon textures to add.</param>
        /// <param name="assetKey16">16 bit Asset IDs.</param>
        /// <param name="atlas">Atlas to add the icons to.</param>
        /// <param name="parent">The object the atlas is linked to.</param>
        public static void AddIconsToAtlas(Texture2D[] icons, int[] assetKey16, IconAtlas atlas, ScriptableObject parent)
        {
            List<Texture2D> texList = new List<Texture2D>();
            texList.AddRange(GetIcons(atlas));
            int tempTexturesCount = texList.Count;

            int index = 0;
            foreach (Texture2D icon in icons)
            {
                if (icon == null)
                    texList.Add(new Texture2D(4, 4));
                else
                {
                    if (!icon.isReadable)
                        texList.Add(AssetTools.CreateReadableTexture(icon, 32, 32));
                    else
                        texList.Add(icon);
                }

                atlas.Add(assetKey16[index], new Rect());
                index++;
            }
            PackIcons(texList.ToArray(), atlas, parent);

            for (int i = 0; i < tempTexturesCount; i++)
                GameObject.DestroyImmediate(texList[i]);

            texList = null;
        }

        /// <summary>
        /// Removes an icon from the atlas.
        /// </summary>
        /// <param name="id16">16 bit asset ID.</param>
        /// <param name="atlas">Atlas to remove the icons from.</param>
        /// <param name="parent">The object the atlas is linked to.</param>
        public static void RemoveIconFromAtlas(int id16, IconAtlas atlas, ScriptableObject parent)
        {
            if (atlas.Contains(id16))
            {
                atlas.Remove(id16);

                Texture2D[] tmpTexs = GetIcons(atlas);
                PackIcons(tmpTexs, atlas, parent);

                foreach (Texture2D tex in tmpTexs)
                    GameObject.DestroyImmediate(tex);
                return;
            }

            Debug.LogWarning("Icon " + id16 + "' not found for removal.");
        }

        /// <summary>
        /// Packs tetures into an atlas.
        /// </summary>
        /// <param name="textures">Textures to pack</param>
        /// <param name="atlas">Atlas that holds the textures.</param>
        /// <param name="parent">The object the atlas is linked to.</param>
        private static void PackIcons(Texture2D[] textures, IconAtlas atlas, ScriptableObject parent)
        {
            Texture2D newTex = new Texture2D(4, 4, TextureFormat.ARGB32, false);
            Rect[] rects = newTex.PackTextures(textures, 0, 2048);
            newTex.Apply();

            for (int i = 0; i < rects.Length; i++)
            {
                atlas.SetUvsAtIndex(i, rects[i]);
            }

            Texture2D finalTexture = new Texture2D(newTex.width, newTex.height, TextureFormat.ARGB32, false);
            finalTexture.SetPixels(newTex.GetPixels());
            finalTexture.Apply();

            string path = AssetDatabase.GetAssetPath(atlas.Texture);

            if (parent != null)
            {
                GameObject.DestroyImmediate(atlas.Texture, true);

                AssetDatabase.AddObjectToAsset(finalTexture, parent);
                finalTexture.name = parent.name + "_Icons";
                atlas.Texture = finalTexture;
                AssetDatabase.ImportAsset(path);
            }
            else
            {
                atlas.Texture = ExportTexture(path, finalTexture, true);
                GameObject.DestroyImmediate(finalTexture);
            }

            GameObject.DestroyImmediate(newTex);
        }

        /// <summary>
        /// Exports a texture to the project.
        /// </summary>
        /// <param name="path">Save path of the texture.</param>
        /// <param name="texture">Texture to save.</param>
        /// <param name="overrite">Should an existing texture be overriden.</param>
        /// <returns>The saved texture.</returns>
        private static Texture2D ExportTexture(string path, Texture2D texture, bool overrite)
        {
            if (!overrite)
                path = AssetDatabase.GenerateUniqueAssetPath(path);

            File.WriteAllBytes(Path.GetFullPath(path), texture.EncodeToPNG());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameObject.DestroyImmediate(texture);
            return ImportTexture(path);
        }

        /// <summary>
        /// Loads a texture in the project.
        /// </summary>
        /// <param name="path">Path to the texture.</param>
        /// <returns>The loaded texture.</returns>
        private static Texture2D ImportTexture(string path)
        {
            SetTextureImportSetting(path, 2048);

            Texture2D tex = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D)) as Texture2D;
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return tex;
        }

        /// <summary>
        /// Sets texture import settings to read
        /// </summary>
        /// <param name="path"></param>
        /// <param name="maxTextureSize"></param>
        private static void SetTextureImportSetting(string path, int maxTextureSize)
        {
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

            TextureImporterSettings settings = new TextureImporterSettings();
            ti.ReadTextureSettings(settings);

            if (!settings.readable)
            {
                settings.readable = true;
                settings.npotScale = TextureImporterNPOTScale.None;
                settings.filterMode = FilterMode.Point;

                ti.SetTextureSettings(settings);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }
        }

        /// <summary>
        /// Gets the icon textures from an atlas.
        /// </summary>
        /// <param name="atlas">The atlas to get textures from.</param>
        /// <returns>A list of textures the given atlas contains.</returns>
        private static Texture2D[] GetIcons(IconAtlas atlas)
        {
            Texture2D[] textures = new Texture2D[atlas.Count];
            Rect entryRect;
            for (int i = 0; i < atlas.Count; i++)
            {
                entryRect = atlas.PixelCoords(atlas.GetKeyAtIndex(i));

                textures[i] = new Texture2D((int)(entryRect.width), (int)(entryRect.height));
                textures[i].SetPixels(atlas.Texture.GetPixels((int)entryRect.x, (int)entryRect.y,
                                                             (int)entryRect.width, (int)entryRect.height));
                textures[i].Apply();
            }
            return textures;
        }
    }
}