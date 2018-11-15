using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YamlDotNet
{
	internal static class ReflectionExtensions
	{
		private static readonly FieldInfo remoteStackTraceField = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);

		public static Type BaseType(this Type type)
		{
			return type.BaseType;
		}

		public static bool IsValueType(this Type type)
		{
			return type.IsValueType;
		}

		public static bool IsGenericType(this Type type)
		{
			return type.IsGenericType;
		}

		public static bool IsInterface(this Type type)
		{
			return type.IsInterface;
		}

		public static bool IsEnum(this Type type)
		{
			return type.IsEnum;
		}

		public static bool IsDbNull(this object value)
		{
			return value is DBNull;
		}

		public static bool HasDefaultConstructor(this Type type)
		{
			return type.IsValueType || type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) != null;
		}

		public static TypeCode GetTypeCode(this Type type)
		{
			return Type.GetTypeCode(type);
		}

		public static PropertyInfo GetPublicProperty(this Type type, string name)
		{
			return type.GetProperty(name);
		}

		public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
		{
			BindingFlags instancePublic = BindingFlags.Instance | BindingFlags.Public;
			return (!type.IsInterface) ? type.GetProperties(instancePublic) : new Type[1]
			{
				type
			}.Concat(type.GetInterfaces()).SelectMany((Type i) => i.GetProperties(instancePublic));
		}

		public static IEnumerable<MethodInfo> GetPublicStaticMethods(this Type type)
		{
			return type.GetMethods(BindingFlags.Static | BindingFlags.Public);
		}

		public static MethodInfo GetPublicStaticMethod(this Type type, string name, params Type[] parameterTypes)
		{
			return type.GetMethod(name, BindingFlags.Static | BindingFlags.Public, null, parameterTypes, null);
		}

		public static MethodInfo GetPublicInstanceMethod(this Type type, string name)
		{
			return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);
		}

		public static Exception Unwrap(this TargetInvocationException ex)
		{
			Exception innerException = ex.InnerException;
			if (remoteStackTraceField != null)
			{
				remoteStackTraceField.SetValue(ex.InnerException, ex.InnerException.StackTrace + "\r\n");
			}
			return innerException;
		}

		public static bool IsInstanceOf(this Type type, object o)
		{
			return type.IsInstanceOfType(o);
		}
	}
}
