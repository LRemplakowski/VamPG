using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("DoorState")]
	public class ES3UserType_DoorsPersistenceData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DoorsPersistenceData() : base(typeof(SunsetSystems.Entities.Interactable.DoorController.DoorsPersistenceData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Entities.Interactable.DoorController.DoorsPersistenceData)obj;
			
			writer.WriteProperty("DoorState", instance.DoorState, ES3Type_bool.Instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Entities.Interactable.DoorController.DoorsPersistenceData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "DoorState":
						instance.DoorState = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Entities.Interactable.DoorController.DoorsPersistenceData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_DoorsPersistenceDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DoorsPersistenceDataArray() : base(typeof(SunsetSystems.Entities.Interactable.DoorController.DoorsPersistenceData[]), ES3UserType_DoorsPersistenceData.Instance)
		{
			Instance = this;
		}
	}
}