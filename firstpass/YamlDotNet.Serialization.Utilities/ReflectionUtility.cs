using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization.Utilities
{
	internal static class ReflectionUtility
	{
		public static Type GetImplementedGenericInterface(Type type, Type genericInterfaceType)
		{
			foreach (Type implementedInterface in GetImplementedInterfaces(type))
			{
				if (implementedInterface.IsGenericType() && implementedInterface.GetGenericTypeDefinition() == genericInterfaceType)
				{
					return implementedInterface;
				}
			}
			return null;
		}

		public static IEnumerable<Type> GetImplementedInterfaces(Type type)
		{
			if (type.IsInterface())
			{
				yield return type;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			Type[] interfaces = type.GetInterfaces();
			int num = 0;
			if (num < interfaces.Length)
			{
				Type implementedInterface = interfaces[num];
				yield return implementedInterface;
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}
	}
}
