using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringQuestDataDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringQuestDataDictionary() : base(typeof(StringQuestDataDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (StringQuestDataDictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (StringQuestDataDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (StringQuestDataDictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (StringQuestDataDictionary)reader.SetPrivateField("m_values", reader.Read<SunsetSystems.Journal.QuestData[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new StringQuestDataDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringQuestDataDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringQuestDataDictionaryArray() : base(typeof(StringQuestDataDictionary[]), ES3UserType_StringQuestDataDictionary.Instance)
		{
			Instance = this;
		}
	}
}