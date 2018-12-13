using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Serialization.TypeInspectors;

namespace YamlDotNet.Serialization
{
	public sealed class YamlAttributeOverridesInspector : TypeInspectorSkeleton
	{
		public sealed class OverridePropertyDescriptor : IPropertyDescriptor
		{
			private readonly IPropertyDescriptor baseDescriptor;

			private readonly YamlAttributeOverrides overrides;

			private readonly Type classType;

			public string Name => baseDescriptor.Name;

			public bool CanWrite => baseDescriptor.CanWrite;

			public Type Type => baseDescriptor.Type;

			public Type TypeOverride
			{
				get
				{
					return baseDescriptor.TypeOverride;
				}
				set
				{
					baseDescriptor.TypeOverride = value;
				}
			}

			public int Order
			{
				get
				{
					return baseDescriptor.Order;
				}
				set
				{
					baseDescriptor.Order = value;
				}
			}

			public ScalarStyle ScalarStyle
			{
				get
				{
					return baseDescriptor.ScalarStyle;
				}
				set
				{
					baseDescriptor.ScalarStyle = value;
				}
			}

			public OverridePropertyDescriptor(IPropertyDescriptor baseDescriptor, YamlAttributeOverrides overrides, Type classType)
			{
				this.baseDescriptor = baseDescriptor;
				this.overrides = overrides;
				this.classType = classType;
			}

			public void Write(object target, object value)
			{
				baseDescriptor.Write(target, value);
			}

			public T GetCustomAttribute<T>() where T : Attribute
			{
				T attribute = overrides.GetAttribute<T>(classType, Name);
				return attribute ?? baseDescriptor.GetCustomAttribute<T>();
			}

			public IObjectDescriptor Read(object target)
			{
				return baseDescriptor.Read(target);
			}
		}

		private readonly ITypeInspector innerTypeDescriptor;

		private readonly YamlAttributeOverrides overrides;

		public YamlAttributeOverridesInspector(ITypeInspector innerTypeDescriptor, YamlAttributeOverrides overrides)
		{
			this.innerTypeDescriptor = innerTypeDescriptor;
			this.overrides = overrides;
		}

		public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
		{
			if (overrides == null)
			{
				return innerTypeDescriptor.GetProperties(type, container);
			}
			return from p in innerTypeDescriptor.GetProperties(type, container)
			select new OverridePropertyDescriptor(p, overrides, type);
		}
	}
}
