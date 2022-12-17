using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringBoolDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringBoolDictionary() : base(typeof(StringBoolDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (StringBoolDictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (StringBoolDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (StringBoolDictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (StringBoolDictionary)reader.SetPrivateField("m_values", reader.Read<System.Boolean[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new StringBoolDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringBoolDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringBoolDictionaryArray() : base(typeof(StringBoolDictionary[]), ES3UserType_StringBoolDictionary.Instance)
		{
			Instance = this;
		}
	}
}