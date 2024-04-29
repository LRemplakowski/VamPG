using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("WorldPosition", "UMAHidden", "GameObjectActive", "PersistentComponentData")]
	public class ES3UserType_CreaturePersistenceData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_CreaturePersistenceData() : base(typeof(SunsetSystems.Entities.Characters.Creature.CreaturePersistenceData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Entities.Characters.Creature.CreaturePersistenceData)obj;
			
			writer.WriteProperty("WorldPosition", instance.WorldPosition, ES3Type_Vector3.Instance);
			writer.WriteProperty("UMAHidden", instance.UMAHidden, ES3Type_bool.Instance);
			writer.WriteProperty("GameObjectActive", instance.GameObjectActive, ES3Type_bool.Instance);
			writer.WriteProperty("PersistentComponentData", instance.PersistentComponentData, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.Object>)));
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Entities.Characters.Creature.CreaturePersistenceData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "WorldPosition":
						instance.WorldPosition = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "UMAHidden":
						instance.UMAHidden = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "GameObjectActive":
						instance.GameObjectActive = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "PersistentComponentData":
						instance.PersistentComponentData = reader.Read<System.Collections.Generic.Dictionary<System.String, System.Object>>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Entities.Characters.Creature.CreaturePersistenceData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_CreaturePersistenceDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CreaturePersistenceDataArray() : base(typeof(SunsetSystems.Entities.Characters.Creature.CreaturePersistenceData[]), ES3UserType_CreaturePersistenceData.Instance)
		{
			Instance = this;
		}
	}
}