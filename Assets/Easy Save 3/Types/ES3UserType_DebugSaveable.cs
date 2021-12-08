using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("someInt", "someFloat", "someVector")]
	public class ES3UserType_DebugSaveable : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DebugSaveable() : base(typeof(DebugTest.DebugSaveable)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (DebugTest.DebugSaveable)obj;
			
			writer.WriteProperty("someInt", instance.someInt, ES3Type_int.Instance);
			writer.WriteProperty("someFloat", instance.someFloat, ES3Type_float.Instance);
			writer.WriteProperty("someVector", instance.someVector, ES3Type_Vector3.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (DebugTest.DebugSaveable)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "someInt":
						instance.someInt = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "someFloat":
						instance.someFloat = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "someVector":
						instance.someVector = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_DebugSaveableArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DebugSaveableArray() : base(typeof(DebugTest.DebugSaveable[]), ES3UserType_DebugSaveable.Instance)
		{
			Instance = this;
		}
	}
}