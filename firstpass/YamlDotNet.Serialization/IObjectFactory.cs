using System;

namespace YamlDotNet.Serialization
{
	public interface IObjectFactory
	{
		object Create(Type type);
	}
}
