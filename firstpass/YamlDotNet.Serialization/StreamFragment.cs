using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization
{
	public sealed class StreamFragment : IYamlConvertible
	{
		private readonly List<ParsingEvent> events = new List<ParsingEvent>();

		public IList<ParsingEvent> Events => events;

		void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
		{
			events.Clear();
			int num = 0;
			while (parser.MoveNext())
			{
				events.Add(parser.Current);
				num += parser.Current.NestingIncrease;
				if (num <= 0)
				{
					return;
				}
			}
			throw new InvalidOperationException("The parser has reached the end before deserialization completed.");
		}

		void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
		{
			foreach (ParsingEvent @event in events)
			{
				emitter.Emit(@event);
			}
		}
	}
}
