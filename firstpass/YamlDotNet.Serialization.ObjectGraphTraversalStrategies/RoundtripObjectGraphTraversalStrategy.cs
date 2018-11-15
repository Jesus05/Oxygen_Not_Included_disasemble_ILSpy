using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace YamlDotNet.Serialization.ObjectGraphTraversalStrategies
{
	public class RoundtripObjectGraphTraversalStrategy : FullObjectGraphTraversalStrategy
	{
		private readonly IEnumerable<IYamlTypeConverter> converters;

		public RoundtripObjectGraphTraversalStrategy(IEnumerable<IYamlTypeConverter> converters, ITypeInspector typeDescriptor, ITypeResolver typeResolver, int maxRecursion)
			: base(typeDescriptor, typeResolver, maxRecursion, null)
		{
			this.converters = converters;
		}

		protected override void TraverseProperties<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, int currentDepth, TContext context)
		{
			if (!value.Type.HasDefaultConstructor() && !converters.Any((IYamlTypeConverter c) => c.Accepts(value.Type)))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Type '{0}' cannot be deserialized because it does not have a default constructor or a type converter.", value.Type));
			}
			base.TraverseProperties(value, visitor, currentDepth, context);
		}
	}
}
