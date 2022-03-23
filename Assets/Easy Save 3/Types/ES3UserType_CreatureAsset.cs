using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_creatureName", "_creatureLastName", "_portrait", "_statsAsset", "_umaPresetFilename", "animatorControllerResourceName", "_creatureFaction", "_bodyType", "_creatureType")]
	public class ES3UserType_CreatureAsset : ES3ScriptableObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CreatureAsset() : base(typeof(Entities.Characters.CreatureAsset)){ Instance = this; priority = 1; }


		protected override void WriteScriptableObject(object obj, ES3Writer writer)
		{
			var instance = (Entities.Characters.CreatureAsset)obj;
			
			writer.WritePrivateField("_creatureName", instance);
			writer.WritePrivateField("_creatureLastName", instance);
			writer.WritePrivateFieldByRef("_portrait", instance);
			writer.WritePrivateFieldByRef("_statsAsset", instance);
			writer.WritePrivateField("_umaPresetFilename", instance);
			writer.WritePrivateField("animatorControllerResourceName", instance);
			writer.WritePrivateField("_creatureFaction", instance);
			writer.WritePrivateField("_bodyType", instance);
			writer.WritePrivateField("_creatureType", instance);
		}

		protected override void ReadScriptableObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Entities.Characters.CreatureAsset)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_creatureName":
					reader.SetPrivateField("_creatureName", reader.Read<System.String>(), instance);
					break;
					case "_creatureLastName":
					reader.SetPrivateField("_creatureLastName", reader.Read<System.String>(), instance);
					break;
					case "_portrait":
					reader.SetPrivateField("_portrait", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "_statsAsset":
					reader.SetPrivateField("_statsAsset", reader.Read<CharacterStats>(), instance);
					break;
					case "_umaPresetFilename":
					reader.SetPrivateField("_umaPresetFilename", reader.Read<System.String>(), instance);
					break;
					case "animatorControllerResourceName":
					reader.SetPrivateField("animatorControllerResourceName", reader.Read<System.String>(), instance);
					break;
					case "_creatureFaction":
					reader.SetPrivateField("_creatureFaction", reader.Read<Faction>(), instance);
					break;
					case "_bodyType":
					reader.SetPrivateField("_bodyType", reader.Read<BodyType>(), instance);
					break;
					case "_creatureType":
					reader.SetPrivateField("_creatureType", reader.Read<CreatureType>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_CreatureAssetArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CreatureAssetArray() : base(typeof(Entities.Characters.CreatureAsset[]), ES3UserType_CreatureAsset.Instance)
		{
			Instance = this;
		}
	}
}