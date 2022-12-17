using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("keys", "values")]
	public class ES3UserType_StringObjectDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringObjectDictionary() : base(typeof(Yarn.Unity.YarnLinesAsCanvasText.StringObjectDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Yarn.Unity.YarnLinesAsCanvasText.StringObjectDictionary)obj;
			
			writer.WritePrivateField("keys", instance);
			writer.WritePrivateField("values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Yarn.Unity.YarnLinesAsCanvasText.StringObjectDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "keys":
					instance = (Yarn.Unity.YarnLinesAsCanvasText.StringObjectDictionary)reader.SetPrivateField("keys", reader.Read<System.Collections.Generic.List<System.String>>(), instance);
					break;
					case "values":
					instance = (Yarn.Unity.YarnLinesAsCanvasText.StringObjectDictionary)reader.SetPrivateField("values", reader.Read<System.Collections.Generic.List<UnityEngine.Object>>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Yarn.Unity.YarnLinesAsCanvasText.StringObjectDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringObjectDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringObjectDictionaryArray() : base(typeof(Yarn.Unity.YarnLinesAsCanvasText.StringObjectDictionary[]), ES3UserType_StringObjectDictionary.Instance)
		{
			Instance = this;
		}
	}
}