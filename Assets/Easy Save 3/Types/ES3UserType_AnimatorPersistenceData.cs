using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("AnimatorStateData")]
	public class ES3UserType_AnimatorPersistenceData : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_AnimatorPersistenceData() : base(typeof(SunsetSystems.Animation.AnimationManager.AnimatorPersistenceData)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Animation.AnimationManager.AnimatorPersistenceData)obj;
			
			writer.WriteProperty("AnimatorStateData", instance.AnimatorStateData, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(System.Collections.Generic.Dictionary<System.Int32, System.Int32>)));
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Animation.AnimationManager.AnimatorPersistenceData)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "AnimatorStateData":
						instance.AnimatorStateData = reader.Read<System.Collections.Generic.Dictionary<System.Int32, System.Int32>>();
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new SunsetSystems.Animation.AnimationManager.AnimatorPersistenceData();
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_AnimatorPersistenceDataArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_AnimatorPersistenceDataArray() : base(typeof(SunsetSystems.Animation.AnimationManager.AnimatorPersistenceData[]), ES3UserType_AnimatorPersistenceData.Instance)
		{
			Instance = this;
		}
	}
}