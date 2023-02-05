using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_selectedWeapon")]
	public class ES3UserType_EquipmentData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_EquipmentData() : base(typeof(SunsetSystems.Entities.Characters.EquipmentData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Entities.Characters.EquipmentData)obj;
			
			writer.WritePrivateField("_selectedWeapon", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Entities.Characters.EquipmentData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_selectedWeapon":
					instance = (SunsetSystems.Entities.Characters.EquipmentData)reader.SetPrivateField("_selectedWeapon", reader.Read<System.String>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Entities.Characters.EquipmentData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_EquipmentDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_EquipmentDataArray() : base(typeof(SunsetSystems.Entities.Characters.EquipmentData[]), ES3UserType_EquipmentData.Instance)
		{
			Instance = this;
		}
	}
}