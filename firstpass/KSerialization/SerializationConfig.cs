using System;

namespace KSerialization
{
	public sealed class SerializationConfig : Attribute
	{
		public MemberSerialization MemberSerialization
		{
			get;
			set;
		}

		public SerializationConfig(MemberSerialization memberSerialization)
		{
			MemberSerialization = memberSerialization;
		}
	}
}
