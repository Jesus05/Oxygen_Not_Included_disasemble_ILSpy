using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.ObjectGraphVisitors
{
	public sealed class CustomSerializationObjectGraphVisitor : ChainedObjectGraphVisitor
	{
		private readonly IEnumerable<IYamlTypeConverter> typeConverters;

		private readonly ObjectSerializer nestedObjectSerializer;

		public CustomSerializationObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor, IEnumerable<IYamlTypeConverter> typeConverters, ObjectSerializer nestedObjectSerializer)
			: base(nextVisitor)
		{
			this.typeConverters = ((typeConverters == null) ? Enumerable.Empty<IYamlTypeConverter>() : typeConverters.ToList());
			this.nestedObjectSerializer = nestedObjectSerializer;
		}

		public override bool Enter(IObjectDescriptor value, IEmitter context)
		{
			IYamlTypeConverter yamlTypeConverter = typeConverters.FirstOrDefault((IYamlTypeConverter t) => t.Accepts(value.Type));
			if (yamlTypeConverter == null)
			{
				IYamlConvertible yamlConvertible = value.Value as IYamlConvertible;
				if (yamlConvertible == null)
				{
					IYamlSerializable yamlSerializable = value.Value as IYamlSerializable;
					if (yamlSerializable == null)
					{
						return base.Enter(value, context);
					}
					yamlSerializable.WriteYaml(context);
					return false;
				}
				yamlConvertible.Write(context, nestedObjectSerializer);
				return false;
			}
			yamlTypeConverter.WriteYaml(context, value.Value, value.Type);
			return false;
		}
	}
}
