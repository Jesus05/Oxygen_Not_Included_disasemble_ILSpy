using System;

namespace YamlDotNet.Serialization
{
	public interface IValuePromise
	{
		event Action<object> ValueAvailable;
	}
}
