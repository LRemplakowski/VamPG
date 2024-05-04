using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("_id")]
	public class ES3UserType_UniqueId : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_UniqueId() : base(typeof(CleverCrow.Fluid.UniqueIds.UniqueId)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (CleverCrow.Fluid.UniqueIds.UniqueId)obj;
			
			writer.WritePrivateField("_id", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (CleverCrow.Fluid.UniqueIds.UniqueId)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "_id":
					reader.SetPrivateField("_id", reader.Read<System.String>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_UniqueIdArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_UniqueIdArray() : base(typeof(CleverCrow.Fluid.UniqueIds.UniqueId[]), ES3UserType_UniqueId.Instance)
		{
			Instance = this;
		}
	}
}