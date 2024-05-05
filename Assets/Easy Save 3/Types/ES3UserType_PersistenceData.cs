using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("GameObjectActive", "PersistentComponentData")]
	public class ES3UserType_PersistenceData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PersistenceData() : base(typeof(SunsetSystems.Entities.PersistentEntity.PersistenceData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Entities.PersistentEntity.PersistenceData)obj;
			
			writer.WriteProperty("GameObjectActive", instance.GameObjectActive, ES3Type_bool.Instance);
			writer.WriteProperty("PersistentComponentData", instance.PersistentComponentData, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.Object>)));
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Entities.PersistentEntity.PersistenceData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "GameObjectActive":
						instance.GameObjectActive = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "PersistentComponentData":
						instance.PersistentComponentData = reader.Read<System.Collections.Generic.Dictionary<System.String, System.Object>>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Entities.PersistentEntity.PersistenceData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_PersistenceDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PersistenceDataArray() : base(typeof(SunsetSystems.Entities.PersistentEntity.PersistenceData[]), ES3UserType_PersistenceData.Instance)
		{
			Instance = this;
		}
	}
}