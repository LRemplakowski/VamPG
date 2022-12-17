using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringIntDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringIntDictionary() : base(typeof(StringIntDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (StringIntDictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (StringIntDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (StringIntDictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (StringIntDictionary)reader.SetPrivateField("m_values", reader.Read<System.Int32[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new StringIntDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringIntDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringIntDictionaryArray() : base(typeof(StringIntDictionary[]), ES3UserType_StringIntDictionary.Instance)
		{
			Instance = this;
		}
	}
}