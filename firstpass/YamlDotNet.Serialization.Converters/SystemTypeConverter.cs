using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.Converters
{
	public class SystemTypeConverter : IYamlTypeConverter
	{
		public bool Accepts(Type type)
		{
			return typeof(Type).IsAssignableFrom(type);
		}

		public object ReadYaml(IParser parser, Type type)
		{
			string value = ((Scalar)parser.Current).Value;
			parser.MoveNext();
			return Type.GetType(value, true);
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			Type type2 = (Type)value;
			emitter.Emit(new Scalar(null, null, type2.AssemblyQualifiedName, ScalarStyle.Any, true, false));
		}
	}
}
