using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringCreatureDataDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringCreatureDataDictionary() : base(typeof(SunsetSystems.Party.StringCreatureDataDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Party.StringCreatureDataDictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Party.StringCreatureDataDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (SunsetSystems.Party.StringCreatureDataDictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (SunsetSystems.Party.StringCreatureDataDictionary)reader.SetPrivateField("m_values", reader.Read<SunsetSystems.Entities.Characters.CreatureData[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Party.StringCreatureDataDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringCreatureDataDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringCreatureDataDictionaryArray() : base(typeof(SunsetSystems.Party.StringCreatureDataDictionary[]), ES3UserType_StringCreatureDataDictionary.Instance)
		{
			Instance = this;
		}
	}
}