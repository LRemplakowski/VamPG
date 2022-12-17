using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringVector3Dictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringVector3Dictionary() : base(typeof(StringVector3Dictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (StringVector3Dictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (StringVector3Dictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (StringVector3Dictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (StringVector3Dictionary)reader.SetPrivateField("m_values", reader.Read<UnityEngine.Vector3[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new StringVector3Dictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringVector3DictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringVector3DictionaryArray() : base(typeof(StringVector3Dictionary[]), ES3UserType_StringVector3Dictionary.Instance)
		{
			Instance = this;
		}
	}
}