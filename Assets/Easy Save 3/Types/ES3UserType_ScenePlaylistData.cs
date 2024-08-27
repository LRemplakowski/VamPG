using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Exploration", "Combat", "Dialogue")]
	public class ES3UserType_ScenePlaylistData : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3UserType_ScenePlaylistData() : base(typeof(SunsetSystems.Core.SceneLoading.ScenePlaylistData)){ Instance = this; priority = 1;}


		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Core.SceneLoading.ScenePlaylistData)obj;
			
			writer.WriteProperty("Exploration", instance.Exploration, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.AddressableAssets.AssetReferenceT<SunsetSystems.Audio.PlaylistConfig>)));
			writer.WriteProperty("Combat", instance.Combat, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.AddressableAssets.AssetReferenceT<SunsetSystems.Audio.PlaylistConfig>)));
			writer.WriteProperty("Dialogue", instance.Dialogue, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.AddressableAssets.AssetReferenceT<SunsetSystems.Audio.PlaylistConfig>)));
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Core.SceneLoading.ScenePlaylistData();
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "Exploration":
						instance.Exploration = reader.Read<UnityEngine.AddressableAssets.AssetReferenceT<SunsetSystems.Audio.PlaylistConfig>>();
						break;
					case "Combat":
						instance.Combat = reader.Read<UnityEngine.AddressableAssets.AssetReferenceT<SunsetSystems.Audio.PlaylistConfig>>();
						break;
					case "Dialogue":
						instance.Dialogue = reader.Read<UnityEngine.AddressableAssets.AssetReferenceT<SunsetSystems.Audio.PlaylistConfig>>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
			return instance;
		}
	}


	public class ES3UserType_ScenePlaylistDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ScenePlaylistDataArray() : base(typeof(SunsetSystems.Core.SceneLoading.ScenePlaylistData[]), ES3UserType_ScenePlaylistData.Instance)
		{
			Instance = this;
		}
	}
}