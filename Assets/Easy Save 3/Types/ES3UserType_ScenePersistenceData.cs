using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("PersistentData")]
	public class ES3UserType_ScenePersistenceData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_ScenePersistenceData() : base(typeof(SunsetSystems.Persistence.ScenePersistenceManager.ScenePersistenceData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Persistence.ScenePersistenceManager.ScenePersistenceData)obj;
			
			writer.WriteProperty("PersistentData", instance.PersistentData, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.Object>)));
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Persistence.ScenePersistenceManager.ScenePersistenceData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "PersistentData":
						instance.PersistentData = reader.Read<System.Collections.Generic.Dictionary<System.String, System.Object>>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Persistence.ScenePersistenceManager.ScenePersistenceData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_ScenePersistenceDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ScenePersistenceDataArray() : base(typeof(SunsetSystems.Persistence.ScenePersistenceManager.ScenePersistenceData[]), ES3UserType_ScenePersistenceData.Instance)
		{
			Instance = this;
		}
	}
}