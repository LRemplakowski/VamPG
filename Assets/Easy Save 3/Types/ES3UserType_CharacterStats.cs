using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_isGeneric", "clan", "generation", "attributes", "skills", "disciplines")]
	public class ES3UserType_CharacterStats : ES3ScriptableObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CharacterStats() : base(typeof(CharacterStats)){ Instance = this; priority = 1; }


		protected override void WriteScriptableObject(object obj, ES3Writer writer)
		{
			var instance = (CharacterStats)obj;
			
			writer.WritePrivateField("_isGeneric", instance);
			writer.WritePrivateField("clan", instance);
			writer.WritePrivateField("generation", instance);
			writer.WritePrivateField("attributes", instance);
			writer.WritePrivateField("skills", instance);
			writer.WritePrivateField("disciplines", instance);
		}

		protected override void ReadScriptableObject<T>(ES3Reader reader, object obj)
		{
			var instance = (CharacterStats)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_isGeneric":
					reader.SetPrivateField("_isGeneric", reader.Read<System.Boolean>(), instance);
					break;
					case "clan":
					reader.SetPrivateField("clan", reader.Read<Clan>(), instance);
					break;
					case "generation":
					reader.SetPrivateField("generation", reader.Read<System.Int32>(), instance);
					break;
					case "attributes":
					reader.SetPrivateField("attributes", reader.Read<CharacterStats.Attributes>(), instance);
					break;
					case "skills":
					reader.SetPrivateField("skills", reader.Read<CharacterStats.Skills>(), instance);
					break;
					case "disciplines":
					reader.SetPrivateField("disciplines", reader.Read<CharacterStats.Disciplines>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_CharacterStatsArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CharacterStatsArray() : base(typeof(CharacterStats[]), ES3UserType_CharacterStats.Instance)
		{
			Instance = this;
		}
	}
}