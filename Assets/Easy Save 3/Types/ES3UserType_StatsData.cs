using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Trackers", "Clan", "Generation", "BloodPotency", "Attributes", "Skills", "Disciplines")]
	public class ES3UserType_StatsData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_StatsData() : base(typeof(SunsetSystems.Entities.Data.StatsData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Entities.Data.StatsData)obj;
			
			writer.WriteProperty("Trackers", instance.Trackers, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(SunsetSystems.Entities.Data.Trackers)));
			writer.WriteProperty("Clan", instance.Clan, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(Clan)));
			writer.WriteProperty("Generation", instance.Generation, ES3Type_int.Instance);
			writer.WriteProperty("BloodPotency", instance.BloodPotency, ES3Type_int.Instance);
			writer.WriteProperty("Attributes", instance.Attributes, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(SunsetSystems.Entities.Data.Attributes)));
			writer.WriteProperty("Skills", instance.Skills, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(SunsetSystems.Entities.Data.Skills)));
			writer.WriteProperty("Disciplines", instance.Disciplines, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(SunsetSystems.Entities.Data.Disciplines)));
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Entities.Data.StatsData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Trackers":
						instance.Trackers = reader.Read<SunsetSystems.Entities.Data.Trackers>();
						break;
					case "Clan":
						instance.Clan = reader.Read<Clan>();
						break;
					case "Generation":
						instance.Generation = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "BloodPotency":
						instance.BloodPotency = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "Attributes":
						instance.Attributes = reader.Read<SunsetSystems.Entities.Data.Attributes>();
						break;
					case "Skills":
						instance.Skills = reader.Read<SunsetSystems.Entities.Data.Skills>();
						break;
					case "Disciplines":
						instance.Disciplines = reader.Read<SunsetSystems.Entities.Data.Disciplines>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Entities.Data.StatsData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_StatsDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_StatsDataArray() : base(typeof(SunsetSystems.Entities.Data.StatsData[]), ES3UserType_StatsData.Instance)
		{
			Instance = this;
		}
	}
}