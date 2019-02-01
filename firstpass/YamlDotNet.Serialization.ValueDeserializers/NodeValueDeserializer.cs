using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization.ValueDeserializers
{
	public sealed class NodeValueDeserializer : IValueDeserializer
	{
		private readonly IList<INodeDeserializer> deserializers;

		private readonly IList<INodeTypeResolver> typeResolvers;

		public NodeValueDeserializer(IList<INodeDeserializer> deserializers, IList<INodeTypeResolver> typeResolvers)
		{
			if (deserializers == null)
			{
				throw new ArgumentNullException("deserializers");
			}
			this.deserializers = deserializers;
			if (typeResolvers == null)
			{
				throw new ArgumentNullException("typeResolvers");
			}
			this.typeResolvers = typeResolvers;
		}

		public object DeserializeValue(IParser parser, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
		{
			NodeEvent nodeEvent = parser.Peek<NodeEvent>();
			Type typeFromEvent = GetTypeFromEvent(nodeEvent, expectedType);
			try
			{
				foreach (INodeDeserializer deserializer in deserializers)
				{
					if (deserializer.Deserialize(parser, typeFromEvent, (IParser r, Type t) => nestedObjectDeserializer.DeserializeValue(r, t, state, nestedObjectDeserializer), out object value))
					{
						return TypeConverter.ChangeType(value, expectedType);
					}
				}
			}
			catch (YamlException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new YamlException(nodeEvent.Start, nodeEvent.End, "Exception during deserialization", innerException);
			}
			throw new YamlException(nodeEvent.Start, nodeEvent.End, $"No node deserializer was able to deserialize the node into type {expectedType.AssemblyQualifiedName}");
		}

		private Type GetTypeFromEvent(NodeEvent nodeEvent, Type currentType)
		{
			foreach (INodeTypeResolver typeResolver in typeResolvers)
			{
				if (typeResolver.Resolve(nodeEvent, ref currentType))
				{
					break;
				}
			}
			return currentType;
		}
	}
}
