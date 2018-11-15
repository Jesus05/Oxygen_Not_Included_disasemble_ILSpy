using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization.ObjectFactories
{
	public sealed class DefaultObjectFactory : IObjectFactory
	{
		private static readonly Dictionary<Type, Type> defaultInterfaceImplementations = new Dictionary<Type, Type>
		{
			{
				typeof(IEnumerable<>),
				typeof(List<>)
			},
			{
				typeof(ICollection<>),
				typeof(List<>)
			},
			{
				typeof(IList<>),
				typeof(List<>)
			},
			{
				typeof(IDictionary<, >),
				typeof(Dictionary<, >)
			}
		};

		public object Create(Type type)
		{
			if (type.IsInterface() && defaultInterfaceImplementations.TryGetValue(type.GetGenericTypeDefinition(), out Type value))
			{
				type = value.MakeGenericType(type.GetGenericArguments());
			}
			try
			{
				return Activator.CreateInstance(type);
			}
			catch (Exception innerException)
			{
				string message = $"Failed to create an instance of type '{type}'.";
				throw new InvalidOperationException(message, innerException);
			}
		}
	}
}
