using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("m_keys", "m_values")]
	public class ES3UserType_StringEquipmentSlotDictionary : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StringEquipmentSlotDictionary() : base(typeof(StringEquipmentSlotDictionary)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (StringEquipmentSlotDictionary)obj;
			
			writer.WritePrivateField("m_keys", instance);
			writer.WritePrivateField("m_values", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (StringEquipmentSlotDictionary)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "m_keys":
					instance = (StringEquipmentSlotDictionary)reader.SetPrivateField("m_keys", reader.Read<System.String[]>(), instance);
					break;
					case "m_values":
					instance = (StringEquipmentSlotDictionary)reader.SetPrivateField("m_values", reader.Read<SunsetSystems.Inventory.EquipmentSlot[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new StringEquipmentSlotDictionary();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StringEquipmentSlotDictionaryArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StringEquipmentSlotDictionaryArray() : base(typeof(StringEquipmentSlotDictionary[]), ES3UserType_StringEquipmentSlotDictionary.Instance)
		{
			Instance = this;
		}
	}
}