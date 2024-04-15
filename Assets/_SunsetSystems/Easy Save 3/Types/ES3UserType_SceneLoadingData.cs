using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("showScenePaths", "scenePaths", "serializationData", "name")]
	public class ES3UserType_SceneLoadingData : ES3ScriptableObjectType
	{
		public static ES3Type Instance = null;
		
		public ES3UserType_SceneLoadingData() : base(typeof(SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset))
		{ 
			Instance = this; 
			priority = 1;
		}

        protected override void WriteScriptableObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset)obj;
			writer.WritePrivateField("showScenePaths", instance);
			writer.WritePrivateField("scenePaths", instance);
			writer.WritePrivateField("serializationData", instance);
			writer.WriteProperty("name", instance.name, ES3Type_string.Instance);
		}

		protected override void ReadScriptableObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "showScenePaths":
					instance = (SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset)reader.SetPrivateField("showScenePaths", reader.Read<System.Boolean>(), instance);
					break;
					case "scenePaths":
					instance = (SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset)reader.SetPrivateField("scenePaths", reader.Read<System.Collections.Generic.List<System.String>>(), instance);
					break;
					case "serializationData":
					instance = (SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset)reader.SetPrivateField("serializationData", reader.Read<Sirenix.Serialization.SerializationData>(), instance);
					break;
					case "name":
						instance.name = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_SceneLoadingDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SceneLoadingDataArray() : base(typeof(SunsetSystems.Core.SceneLoading.SceneLoadingDataAsset[]), ES3UserType_SceneLoadingData.Instance)
		{
			Instance = this;
		}
	}
}