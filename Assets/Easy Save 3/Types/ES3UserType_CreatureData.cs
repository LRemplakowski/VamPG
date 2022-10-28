using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_dataInstance", "data", "_firstName", "_lastName", "_portrait", "_faction", "_bodyType", "_creatureType")]
	public class ES3UserType_CreatureData : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CreatureData() : base(typeof(SunsetSystems.Entities.Characters.CreatureData)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Entities.Characters.CreatureData)obj;
			
			writer.WritePrivateFieldByRef("_dataInstance", instance);
			writer.WritePrivateFieldByRef("data", instance);
			writer.WritePrivateField("_firstName", instance);
			writer.WritePrivateField("_lastName", instance);
			writer.WritePrivateFieldByRef("_portrait", instance);
			writer.WritePrivateField("_faction", instance);
			writer.WritePrivateField("_bodyType", instance);
			writer.WritePrivateField("_creatureType", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Entities.Characters.CreatureData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_dataInstance":
					reader.SetPrivateField("_dataInstance", reader.Read<SunsetSystems.Entities.Characters.CreatureConfig>(), instance);
					break;
					case "data":
					reader.SetPrivateField("data", reader.Read<SunsetSystems.Entities.Characters.CreatureConfig>(), instance);
					break;
					case "_firstName":
					reader.SetPrivateField("_firstName", reader.Read<System.String>(), instance);
					break;
					case "_lastName":
					reader.SetPrivateField("_lastName", reader.Read<System.String>(), instance);
					break;
					case "_portrait":
					reader.SetPrivateField("_portrait", reader.Read<UnityEngine.Sprite>(), instance);
					break;
					case "_faction":
					reader.SetPrivateField("_faction", reader.Read<Faction>(), instance);
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


	public class ES3UserType_CreatureDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CreatureDataArray() : base(typeof(SunsetSystems.Entities.Characters.CreatureData[]), ES3UserType_CreatureData.Instance)
		{
			Instance = this;
		}
	}
}