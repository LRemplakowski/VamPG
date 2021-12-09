using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("creaturePrefab")]
	public class ES3UserType_GameJournal : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_GameJournal() : base(typeof(SunsetSystems.GameData.GameData)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.GameData.GameData)obj;
			
			writer.WritePrivateFieldByRef("creaturePrefab", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.GameData.GameData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "creaturePrefab":
					reader.SetPrivateField("creaturePrefab", reader.Read<Entities.Characters.CreatureData>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_GameJournalArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_GameJournalArray() : base(typeof(SunsetSystems.GameData.GameData[]), ES3UserType_GameJournal.Instance)
		{
			Instance = this;
		}
	}
}