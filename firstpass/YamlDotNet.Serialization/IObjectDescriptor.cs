using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	public interface IObjectDescriptor
	{
		object Value
		{
			get;
		}

		Type Type
		{
			get;
		}

		Type StaticType
		{
			get;
		}

		ScalarStyle ScalarStyle
		{
			get;
		}
	}
}
