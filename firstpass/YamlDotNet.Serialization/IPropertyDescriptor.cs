using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	public interface IPropertyDescriptor
	{
		string Name
		{
			get;
		}

		bool CanWrite
		{
			get;
		}

		Type Type
		{
			get;
		}

		Type TypeOverride
		{
			get;
			set;
		}

		int Order
		{
			get;
			set;
		}

		ScalarStyle ScalarStyle
		{
			get;
			set;
		}

		T GetCustomAttribute<T>() where T : Attribute;

		IObjectDescriptor Read(object target);

		void Write(object target, object value);
	}
}
