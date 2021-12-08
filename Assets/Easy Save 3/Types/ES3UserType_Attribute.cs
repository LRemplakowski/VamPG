using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("baseValue", "attributeType")]
	public class ES3UserType_Attribute : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Attribute() : base(typeof(Attribute)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Attribute)obj;
			
			writer.WritePrivateField("baseValue", instance);
			writer.WritePrivateField("attributeType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Attribute)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "baseValue":
					reader.SetPrivateField("baseValue", reader.Read<System.Int32>(), instance);
					break;
					case "attributeType":
					reader.SetPrivateField("attributeType", reader.Read<AttributeType>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Attribute(AttributeType.Invalid);
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_AttributeArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AttributeArray() : base(typeof(Attribute[]), ES3UserType_Attribute.Instance)
		{
			Instance = this;
		}
	}
}