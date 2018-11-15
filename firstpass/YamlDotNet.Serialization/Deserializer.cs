using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization.Utilities;

namespace YamlDotNet.Serialization
{
	public sealed class Deserializer
	{
		private readonly IValueDeserializer valueDeserializer;

		public Deserializer()
			: this(new DeserializerBuilder().BuildValueDeserializer())
		{
		}

		private Deserializer(IValueDeserializer valueDeserializer)
		{
			if (valueDeserializer == null)
			{
				throw new ArgumentNullException("valueDeserializer");
			}
			this.valueDeserializer = valueDeserializer;
		}

		public static Deserializer FromValueDeserializer(IValueDeserializer valueDeserializer)
		{
			return new Deserializer(valueDeserializer);
		}

		public T Deserialize<T>(string input)
		{
			using (StringReader input2 = new StringReader(input))
			{
				return (T)Deserialize(input2, typeof(T));
			}
		}

		public T Deserialize<T>(TextReader input)
		{
			return (T)Deserialize(input, typeof(T));
		}

		public object Deserialize(TextReader input)
		{
			return Deserialize(input, typeof(object));
		}

		public object Deserialize(string input, Type type)
		{
			using (StringReader input2 = new StringReader(input))
			{
				return Deserialize(input2, type);
			}
		}

		public object Deserialize(TextReader input, Type type)
		{
			return Deserialize(new Parser(input), type);
		}

		public T Deserialize<T>(IParser parser)
		{
			return (T)Deserialize(parser, typeof(T));
		}

		public object Deserialize(IParser parser)
		{
			return Deserialize(parser, typeof(object));
		}

		public object Deserialize(IParser parser, Type type)
		{
			if (parser == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			bool flag = parser.Allow<StreamStart>() != null;
			bool flag2 = parser.Allow<DocumentStart>() != null;
			object result = null;
			if (!parser.Accept<DocumentEnd>() && !parser.Accept<StreamEnd>())
			{
				using (SerializerState serializerState = new SerializerState())
				{
					result = valueDeserializer.DeserializeValue(parser, type, serializerState, valueDeserializer);
					serializerState.OnDeserialization();
				}
			}
			if (flag2)
			{
				parser.Expect<DocumentEnd>();
			}
			if (flag)
			{
				parser.Expect<StreamEnd>();
			}
			return result;
		}
	}
}
