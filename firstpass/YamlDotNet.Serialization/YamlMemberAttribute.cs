using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class YamlMemberAttribute : Attribute
	{
		public Type SerializeAs
		{
			get;
			set;
		}

		public int Order
		{
			get;
			set;
		}

		public string Alias
		{
			get;
			set;
		}

		public bool ApplyNamingConventions
		{
			get;
			set;
		}

		public ScalarStyle ScalarStyle
		{
			get;
			set;
		}

		public YamlMemberAttribute()
		{
			ScalarStyle = ScalarStyle.Any;
			ApplyNamingConventions = true;
		}

		public YamlMemberAttribute(Type serializeAs)
			: this()
		{
			SerializeAs = serializeAs;
		}
	}
}
