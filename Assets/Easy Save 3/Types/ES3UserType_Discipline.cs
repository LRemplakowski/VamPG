using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("baseValue", "disciplineType", "knownPowers")]
	public class ES3UserType_Discipline : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Discipline() : base(typeof(Discipline)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Discipline)obj;
			
			writer.WritePrivateField("baseValue", instance);
			writer.WritePrivateField("disciplineType", instance);
			writer.WritePrivateField("knownPowers", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Discipline)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "baseValue":
					reader.SetPrivateField("baseValue", reader.Read<System.Int32>(), instance);
					break;
					case "disciplineType":
					reader.SetPrivateField("disciplineType", reader.Read<DisciplineType>(), instance);
					break;
					case "knownPowers":
					reader.SetPrivateField("knownPowers", reader.Read<DisciplinePower[]>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Discipline(DisciplineType.Invalid);
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_DisciplineArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DisciplineArray() : base(typeof(Discipline[]), ES3UserType_Discipline.Instance)
		{
			Instance = this;
		}
	}
}