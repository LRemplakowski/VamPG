using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("<DatabaseID>k__BackingField", "<ReadableID>k__BackingField", "<FirstName>k__BackingField", "<LastName>k__BackingField", "<Faction>k__BackingField", "<BodyType>k__BackingField", "<CreatureType>k__BackingField", "<PortraitAssetRef>k__BackingField", "<BaseLookWardrobeCollection>k__BackingField", "<EquipmentSlotsData>k__BackingField", "<StatsData>k__BackingField")]
	public class ES3UserType_TemplateFromCreatureAsset : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TemplateFromCreatureAsset() : base(typeof(SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)obj;
			
			writer.WritePrivateField("<DatabaseID>k__BackingField", instance);
			writer.WritePrivateField("<ReadableID>k__BackingField", instance);
			writer.WritePrivateField("<FirstName>k__BackingField", instance);
			writer.WritePrivateField("<LastName>k__BackingField", instance);
			writer.WritePrivateField("<Faction>k__BackingField", instance);
			writer.WritePrivateField("<BodyType>k__BackingField", instance);
			writer.WritePrivateField("<CreatureType>k__BackingField", instance);
			writer.WritePrivateField("<PortraitAssetRef>k__BackingField", instance);
			writer.WritePrivateFieldByRef("<BaseLookWardrobeCollection>k__BackingField", instance);
			writer.WritePrivateField("<EquipmentSlotsData>k__BackingField", instance);
			writer.WritePrivateField("<StatsData>k__BackingField", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "<DatabaseID>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<DatabaseID>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "<ReadableID>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<ReadableID>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "<FirstName>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<FirstName>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "<LastName>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<LastName>k__BackingField", reader.Read<System.String>(), instance);
					break;
					case "<Faction>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<Faction>k__BackingField", reader.Read<Faction>(), instance);
					break;
					case "<BodyType>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<BodyType>k__BackingField", reader.Read<SunsetSystems.Entities.Characters.BodyType>(), instance);
					break;
					case "<CreatureType>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<CreatureType>k__BackingField", reader.Read<CreatureType>(), instance);
					break;
					case "<PortraitAssetRef>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<PortraitAssetRef>k__BackingField", reader.Read<UnityEngine.AddressableAssets.AssetReferenceSprite>(), instance);
					break;
					case "<BaseLookWardrobeCollection>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<BaseLookWardrobeCollection>k__BackingField", reader.Read<UMA.CharacterSystem.UMAWardrobeCollection>(), instance);
					break;
					case "<EquipmentSlotsData>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<EquipmentSlotsData>k__BackingField", reader.Read<System.Collections.Generic.Dictionary<SunsetSystems.Equipment.EquipmentSlotID, System.String>>(), instance);
					break;
					case "<StatsData>k__BackingField":
					instance = (SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset)reader.SetPrivateField("<StatsData>k__BackingField", reader.Read<SunsetSystems.Entities.Data.StatsData>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_TemplateFromCreatureAssetArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TemplateFromCreatureAssetArray() : base(typeof(SunsetSystems.Entities.Characters.CreatureConfig.TemplateFromCreatureAsset[]), ES3UserType_TemplateFromCreatureAsset.Instance)
		{
			Instance = this;
		}
	}
}