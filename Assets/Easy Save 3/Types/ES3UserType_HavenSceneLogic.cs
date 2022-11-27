using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	public class ES3UserType_HavenSceneLogic : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_HavenSceneLogic() : base(typeof(SunsetSystems.Loading.HavenSceneLogic)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (SunsetSystems.Loading.HavenSceneLogic)obj;
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (SunsetSystems.Loading.HavenSceneLogic)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
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