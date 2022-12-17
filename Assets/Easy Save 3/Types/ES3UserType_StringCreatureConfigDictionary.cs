using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringCreatureConfigDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringCreatureConfigDictionary() : base(typeof(StringCreatureConfigDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (StringCreatureConfigDictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (StringCreatureConfigDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (StringCreatureConfigDictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (StringCreatureConfigDictionary)reader.SetPrivateField("m_values", reader.Read<SunsetSystems.Entities.Characters.CreatureConfig[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new StringCreatureConfigDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringCreatureConfigDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringCreatureConfigDictionaryArray() : base(typeof(StringCreatureConfigDictionary[]), ES3UserType_StringCreatureConfigDictionary.Instance)
		{
			Instance = this;
		}
	}
}