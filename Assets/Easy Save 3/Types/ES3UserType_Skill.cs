using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("baseValue", "skillType")]
	public class ES3UserType_Skill : ES3ObjectType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Skill() : base(typeof(Skill)){ Instance = this; priority = 1; }


		protected override void WriteObject(object obj, ES3Writer writer)
		{
			var instance = (Skill)obj;
			
			writer.WritePrivateField("baseValue", instance);
			writer.WritePrivateField("skillType", instance);
		}

		protected override void ReadObject<T>(ES3Reader reader, object obj)
		{
			var instance = (Skill)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "baseValue":
					reader.SetPrivateField("baseValue", reader.Read<System.Int32>(), instance);
					break;
					case "skillType":
					reader.SetPrivateField("skillType", reader.Read<SkillType>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}

		protected override object ReadObject<T>(ES3Reader reader)
		{
			var instance = new Skill(SkillType.Invalid);
			ReadObject<T>(reader, instance);
			return instance;
		}
	}


	public class ES3UserType_SkillArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_SkillArray() : base(typeof(Skill[]), ES3UserType_Skill.Instance)
		{
			Instance = this;
		}
	}
}