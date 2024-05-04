using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("ScenePaths")]
	public class ES3UserType_LevelLoadingData : ES3Type
	{
		public static ES3Type Instance = null;

		public ES3UserType_LevelLoadingData() : base(typeof(SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset.LevelLoadingData)){ Instance = this; priority = 1;}


		public override void Write(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset.LevelLoadingData)obj;
			
			writer.WriteProperty("ScenePaths", instance.ScenePaths, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.String>)));
		}

		public override object Read<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset.LevelLoadingData();
			string propertyName;
			while((propertyName = reader.ReadPropertyName()) != null)
			{
				switch(propertyName)
				{
					
					case "ScenePaths":
						instance.ScenePaths = reader.Read<System.Collections.Generic.List<System.String>>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
			return instance;
		}
	}


	public class ES3UserType_LevelLoadingDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_LevelLoadingDataArray() : base(typeof(SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset.LevelLoadingData[]), ES3UserType_LevelLoadingData.Instance)
		{
			Instance = this;
		}
	}
}