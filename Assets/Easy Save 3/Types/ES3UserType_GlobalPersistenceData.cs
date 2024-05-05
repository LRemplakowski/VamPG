using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_persistenceDataDictionary")]
	public class ES3UserType_GlobalPersistenceData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_GlobalPersistenceData() : base(typeof(SunsetSystems.Persistence.GlobalPersistenceData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Persistence.GlobalPersistenceData)obj;
			
			writer.WritePrivateField("_persistenceDataDictionary", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Persistence.GlobalPersistenceData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_persistenceDataDictionary":
					instance = (SunsetSystems.Persistence.GlobalPersistenceData)reader.SetPrivateField("_persistenceDataDictionary", reader.Read<System.Collections.Generic.Dictionary<System.String, System.Object>>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Persistence.GlobalPersistenceData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_GlobalPersistenceDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_GlobalPersistenceDataArray() : base(typeof(SunsetSystems.Persistence.GlobalPersistenceData[]), ES3UserType_GlobalPersistenceData.Instance)
		{
			Instance = this;
		}
	}
}