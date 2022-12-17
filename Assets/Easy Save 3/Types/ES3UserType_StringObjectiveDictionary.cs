using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringObjectiveDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringObjectiveDictionary() : base(typeof(StringObjectiveDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (StringObjectiveDictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (StringObjectiveDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (StringObjectiveDictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (StringObjectiveDictionary)reader.SetPrivateField("m_values", reader.Read<SunsetSystems.Journal.Objective[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new StringObjectiveDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringObjectiveDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringObjectiveDictionaryArray() : base(typeof(StringObjectiveDictionary[]), ES3UserType_StringObjectiveDictionary.Instance)
		{
			Instance = this;
		}
	}
}