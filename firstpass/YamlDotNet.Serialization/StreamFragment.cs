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
			do
			{
				if (!parser.MoveNext())
				{
					throw new InvalidOperationException("The parser has reached the end before deserialization completed.");
				}
				events.Add(parser.Current);
				num += parser.Current.NestingIncrease;
			}
			while (num > 0);
			Debug.Assert(num == 0);
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
