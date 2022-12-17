using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringQuestDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringQuestDictionary() : base(typeof(StringQuestDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (StringQuestDictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (StringQuestDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (StringQuestDictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (StringQuestDictionary)reader.SetPrivateField("m_values", reader.Read<SunsetSystems.Journal.Quest[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new StringQuestDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringQuestDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringQuestDictionaryArray() : base(typeof(StringQuestDictionary[]), ES3UserType_StringQuestDictionary.Instance)
		{
			Instance = this;
		}
	}
}