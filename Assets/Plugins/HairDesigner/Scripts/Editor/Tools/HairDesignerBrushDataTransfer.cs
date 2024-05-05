using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Kalagaan.HairDesignerExtension
{

	public class HairDesignerBrushDataTransferWindow : EditorWindow
	{
		public static Texture2D m_icon;

		Mesh m_MeshRef;
		Mesh m_MeshTarget;
		Texture2D m_brushTextureRef;


		[MenuItem("Window/HairDesigner/Brush data transfer")]
		static public HairDesignerBrushDataTransferWindow Init()
		{
			// Get existing open window or if none, make a new one:
			HairDesignerBrushDataTransferWindow window = (HairDesignerBrushDataTransferWindow)EditorWindow.GetWindow(typeof(HairDesignerBrushDataTransferWindow));
			m_icon = Resources.Load<Texture2D>("Icons/BULLET");
			window.titleContent = new GUIContent("Brush data transfer", m_icon);
			window.Show();
			return window;
		}


		// Use this for initialization
		void OnGUI()
		{
			GUILayout.Label("Fur Layer : Brush texture transfer tool", EditorStyles.boldLabel);

			EditorGUILayout.HelpBox(
				"Use this tool if the mesh structure has been modified (vertex count or vertex order)."
				+ " You need the original version of the mesh used for generating the brush texture."
				, MessageType.Info); ;

			
			//GUILayout.Label("Old mesh")
			GUILayout.BeginVertical(EditorStyles.helpBox);
			m_MeshRef = EditorGUILayout.ObjectField("Source mesh", m_MeshRef, typeof(Mesh), false) as Mesh;
			m_brushTextureRef = EditorGUILayout.ObjectField("Source texture", m_brushTextureRef, typeof(Texture2D), false) as Texture2D;
			GUILayout.EndVertical();

			GUILayout.BeginVertical(EditorStyles.helpBox);
			m_MeshTarget = EditorGUILayout.ObjectField("Target mesh", m_MeshTarget, typeof(Mesh), false) as Mesh;
			GUILayout.EndVertical();

			if (GUILayout.Button("Transfer"))
			{
				Transfert();
			}




		}

		// Update is called once per frame
		void Transfert()
		{

			int width = 2048;
			int height = 2048;

			string filename = "";
			string texturePath = "";
			Color c = Color.white;

			filename = m_brushTextureRef.name + "_transfer";
			texturePath = AssetDatabase.GetAssetPath(m_brushTextureRef);
			c = new Color(1f, .5f, .5f, .99f);
			width = height = 1024;

			int vCount = m_MeshTarget.vertexCount;


			if (vCount > 0)
			{
				//set the minimum size for the texture
				if (vCount <= 1024 * 1024) width = height = 1024;
				if (vCount <= 512 * 512) width = height = 512;
				if (vCount <= 256 * 256) width = height = 256;
				if (vCount <= 128 * 128) width = height = 128;
				if (vCount <= 64 * 64) width = height = 64;
				if (vCount <= 32 * 32) width = height = 32;
				if (vCount <= 16 * 16) width = height = 16;
				if (vCount <= 8 * 8) width = height = 8;
			}


			string path = EditorUtility.SaveFilePanel(
				"Save texture as PNG",
				texturePath,
				filename + ".png",
				"png");

			if (path.Length == 0)
				return;


			Texture2D image = new Texture2D(width, height, TextureFormat.ARGB32, true, true);
			for (int x = 0; x < image.width; ++x)
				for (int y = 0; y < image.height; ++y)
				{
					image.SetPixel(x, y, c);
				}

			TransferData(ref image);
			image.Apply();



			byte[] bytes = image.EncodeToPNG();
			System.IO.File.WriteAllBytes(path, bytes);
			AssetDatabase.Refresh();


			path = path.Replace(Application.dataPath, "Assets");


			TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
			ImporterSettings(ref importer);

			importer.textureType = TextureImporterType.Default;
			importer.wrapMode = TextureWrapMode.Clamp;
			//importer.textureFormat = TextureImporterFormat.ARGB32;
			importer.mipmapEnabled = false;
			importer.compressionQuality = 0;

			importer.isReadable = true;
			importer.SaveAndReimport();
		}



		public void ImporterSettings(ref TextureImporter importer)
		{
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.sRGBTexture = false;
			importer.alphaIsTransparency = true;
			importer.alphaSource = TextureImporterAlphaSource.FromInput;
		}



		void TransferData(ref Texture2D target)
		{
			//Color empty = new Color(1f, .5f, .5f, .99f);
			Color c = Color.white;
			Vector2 uv = Vector2.zero;
			Dictionary<Vector2, Color> dic = new Dictionary<Vector2, Color>();

			Vector2[] srcUVs = m_MeshRef.uv;
			Vector2[] destUVs = m_MeshTarget.uv;

			//generate the transfer dictionary
			for (  int vid=0; vid<m_MeshRef.vertexCount; ++vid)
			{				
				uv.x = (float)(vid) / (float)m_brushTextureRef.width;
				uv.y = Mathf.Floor(uv.x) / (float)m_brushTextureRef.width;
				uv.x -= Mathf.Floor(uv.x);
				float halfPixel = (1f / (float)m_brushTextureRef.width) * .5f;
				int cx = (int)((uv.x + halfPixel) * (float)m_brushTextureRef.width);
				int cy = (int)((1 - uv.y - halfPixel) * (float)m_brushTextureRef.height);

				if (!dic.ContainsKey(srcUVs[vid]))
					dic.Add(srcUVs[vid], m_brushTextureRef.GetPixel(cx, cy));
			}


			for (int vid = 0; vid < m_MeshTarget.vertexCount; ++vid)
			{
				uv.x = (float)(vid) / (float)target.width;
				uv.y = Mathf.Floor(uv.x) / (float)target.width;
				uv.x -= Mathf.Floor(uv.x);
				float halfPixel = (1f / (float)target.width) * .5f;
				int cx = (int)((uv.x + halfPixel) * (float)target.width);
				int cy = (int)((1 - uv.y - halfPixel) * (float)target.height);

				float maxDist = float.MaxValue;
				foreach( KeyValuePair<Vector2, Color> kvp in dic )
				{
					uv = destUVs[vid];
					float d = Vector2.Distance(kvp.Key, uv);
					if ( d < maxDist)
					{
						maxDist = d;
						c = kvp.Value;
					}
				}
							   
				target.SetPixel(cx, cy, c);
			}

			dic.Clear();
		}


	}


}