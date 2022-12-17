using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringFloatDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringFloatDictionary() : base(typeof(StringFloatDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (StringFloatDictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (StringFloatDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (StringFloatDictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (StringFloatDictionary)reader.SetPrivateField("m_values", reader.Read<System.Single[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new StringFloatDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringFloatDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringFloatDictionaryArray() : base(typeof(StringFloatDictionary[]), ES3UserType_StringFloatDictionary.Instance)
		{
			Instance = this;
		}
	}
}