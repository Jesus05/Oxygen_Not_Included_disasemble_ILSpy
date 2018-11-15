using System;

namespace YamlDotNet.Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class YamlIgnoreAttribute : Attribute
	{
	}
}
