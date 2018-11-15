using System;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization
{
	public sealed class Serializer
	{
		private readonly IValueSerializer valueSerializer;

		public Serializer()
			: this(new SerializerBuilder().BuildValueSerializer())
		{
		}

		private Serializer(IValueSerializer valueSerializer)
		{
			if (valueSerializer == null)
			{
				throw new ArgumentNullException("valueSerializer");
			}
			this.valueSerializer = valueSerializer;
		}

		public static Serializer FromValueSerializer(IValueSerializer valueSerializer)
		{
			return new Serializer(valueSerializer);
		}

		public void Serialize(TextWriter writer, object graph)
		{
			Serialize(new Emitter(writer), graph);
		}

		public string Serialize(object graph)
		{
			using (StringWriter stringWriter = new StringWriter())
			{
				Serialize(stringWriter, graph);
				return stringWriter.ToString();
			}
		}

		public void Serialize(TextWriter writer, object graph, Type type)
		{
			Serialize(new Emitter(writer), graph, type);
		}

		public void Serialize(IEmitter emitter, object graph)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			EmitDocument(emitter, graph, null);
		}

		public void Serialize(IEmitter emitter, object graph, Type type)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			EmitDocument(emitter, graph, type);
		}

		private void EmitDocument(IEmitter emitter, object graph, Type type)
		{
			emitter.Emit(new StreamStart());
			emitter.Emit(new DocumentStart());
			valueSerializer.SerializeValue(emitter, graph, type);
			emitter.Emit(new DocumentEnd(true));
			emitter.Emit(new StreamEnd());
		}
	}
}
