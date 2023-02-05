using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("FirstName", "LastName", "PortraitFileName", "Faction", "BodyType", "CreatureType", "UmaPresetFileName", "AnimatorControllerAsset", "UseEquipmentPreset", "Money")]
	public class ES3UserType_CreatureData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CreatureData() : base(typeof(SunsetSystems.Entities.Characters.CreatureData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Entities.Characters.CreatureData)obj;
			
			writer.WriteProperty("FirstName", instance.FirstName, ES3Type_string.Instance);
			writer.WriteProperty("LastName", instance.LastName, ES3Type_string.Instance);
			writer.WriteProperty("PortraitFileName", instance.PortraitFileName, ES3Type_string.Instance);
			writer.WriteProperty("Faction", instance.Faction, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Faction)));
			writer.WriteProperty("BodyType", instance.BodyType, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(BodyType)));
			writer.WriteProperty("CreatureType", instance.CreatureType, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(CreatureType)));
			writer.WriteProperty("UmaPresetFileName", instance.UmaPresetFileName, ES3Type_string.Instance);
			writer.WritePropertyByRef("AnimatorControllerAsset", instance.AnimatorControllerAsset);
			writer.WriteProperty("UseEquipmentPreset", instance.UseEquipmentPreset, ES3Type_bool.Instance);
			writer.WriteProperty("Money", instance.Money, ES3Type_float.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Entities.Characters.CreatureData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "FirstName":
						instance.FirstName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "LastName":
						instance.LastName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "PortraitFileName":
						instance.PortraitFileName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "Faction":
						instance.Faction = reader.Read<Faction>();
						break;
					case "BodyType":
						instance.BodyType = reader.Read<BodyType>();
						break;
					case "CreatureType":
						instance.CreatureType = reader.Read<CreatureType>();
						break;
					case "UmaPresetFileName":
						instance.UmaPresetFileName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "AnimatorControllerAsset":
						instance.AnimatorControllerAsset = reader.Read<UnityEngine.RuntimeAnimatorController>();
						break;
					case "UseEquipmentPreset":
						instance.UseEquipmentPreset = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "Money":
						instance.Money = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Entities.Characters.CreatureData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_CreatureDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CreatureDataArray() : base(typeof(SunsetSystems.Entities.Characters.CreatureData[]), ES3UserType_CreatureData.Instance)
		{
			Instance = this;
		}
	}
}