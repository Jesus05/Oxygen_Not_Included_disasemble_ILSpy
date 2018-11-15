using System;
using System.Collections.Generic;
using System.ComponentModel;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class DefaultExclusiveObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		private static readonly IEqualityComparer<object> _objectComparer = EqualityComparer<object>.Default;

		public DefaultExclusiveObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
			: base(nextVisitor)
		{
		}

		private static object GetDefault(Type type)
		{
			return (!type.IsValueType()) ? null : Activator.CreateInstance(type);
		}

		public override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			return !_objectComparer.Equals(value, GetDefault(value.Type)) && base.EnterMapping(key, value, context);
		}

		public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
		{
			DefaultValueAttribute customAttribute = key.GetCustomAttribute<DefaultValueAttribute>();
			object y = (customAttribute == null) ? GetDefault(key.Type) : customAttribute.Value;
			return !_objectComparer.Equals(value.Value, y) && base.EnterMapping(key, value, context);
		}
	}
}
