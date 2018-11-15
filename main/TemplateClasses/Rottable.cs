using Klei;
using System;

namespace TemplateClasses
{
	[Serializable]
	public class Rottable : YamlIO<Rottable>
	{
		public float rotAmount
		{
			get;
			set;
		}
	}
}
