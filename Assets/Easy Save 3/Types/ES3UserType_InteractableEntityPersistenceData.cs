using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("Interactable", "Interacted", "InteractableOnce", "InteractionHandlers", "GameObjectActive", "PersistentComponentData")]
	public class ES3UserType_InteractableEntityPersistenceData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_InteractableEntityPersistenceData() : base(typeof(SunsetSystems.Entities.Interactable.InteractableEntity.InteractableEntityPersistenceData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Entities.Interactable.InteractableEntity.InteractableEntityPersistenceData)obj;
			
			writer.WriteProperty("Interactable", instance.Interactable, ES3Type_bool.Instance);
			writer.WriteProperty("Interacted", instance.Interacted, ES3Type_bool.Instance);
			writer.WriteProperty("InteractableOnce", instance.InteractableOnce, ES3Type_bool.Instance);
			writer.WriteProperty("InteractionHandlers", instance.InteractionHandlers, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.List<System.Int64>)));
			writer.WriteProperty("GameObjectActive", instance.GameObjectActive, ES3Type_bool.Instance);
			writer.WriteProperty("PersistentComponentData", instance.PersistentComponentData, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.Dictionary<System.String, System.Object>)));
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Entities.Interactable.InteractableEntity.InteractableEntityPersistenceData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "Interactable":
						instance.Interactable = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "Interacted":
						instance.Interacted = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "InteractableOnce":
						instance.InteractableOnce = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "InteractionHandlers":
						instance.InteractionHandlers = reader.Read<System.Collections.Generic.List<System.Int64>>();
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
			var instance = new SunsetSystems.Entities.Interactable.InteractableEntity.InteractableEntityPersistenceData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_InteractableEntityPersistenceDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_InteractableEntityPersistenceDataArray() : base(typeof(SunsetSystems.Entities.Interactable.InteractableEntity.InteractableEntityPersistenceData[]), ES3UserType_InteractableEntityPersistenceData.Instance)
		{
			Instance = this;
		}
	}
}