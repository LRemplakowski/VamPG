using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("someInt", "someString", "someFloat", "someBool")]
	public class ES3UserType_HavenSceneLogic : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_HavenSceneLogic() : base(typeof(SunsetSystems.Loading.HavenSceneLogic)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Loading.HavenSceneLogic)obj;
			
			writer.WriteProperty("someInt", instance.someInt, ES3Type_int.Instance);
			writer.WriteProperty("someString", instance.someString, ES3Type_string.Instance);
			writer.WriteProperty("someFloat", instance.someFloat, ES3Type_float.Instance);
			writer.WriteProperty("someBool", instance.someBool, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Loading.HavenSceneLogic)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "someInt":
						instance.someInt = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "someString":
						instance.someString = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "someFloat":
						instance.someFloat = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "someBool":
						instance.someBool = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_HavenSceneLogicArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_HavenSceneLogicArray() : base(typeof(SunsetSystems.Loading.HavenSceneLogic[]), ES3UserType_HavenSceneLogic.Instance)
		{
			Instance = this;
		}
	}
}