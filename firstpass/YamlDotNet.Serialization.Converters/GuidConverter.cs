using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.Converters
{
	public class GuidConverter : IYamlTypeConverter
	{
		private readonly bool jsonCompatible;

		public GuidConverter(bool jsonCompatible)
		{
			this.jsonCompatible = jsonCompatible;
		}

		public bool Accepts(Type type)
		{
			return type == typeof(Guid);
		}

		public object ReadYaml(IParser parser, Type type)
		{
			string value = ((Scalar)parser.Current).Value;
			parser.MoveNext();
			return new Guid(value);
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			emitter.Emit(new Scalar(null, null, ((Guid)value).ToString("D"), jsonCompatible ? ScalarStyle.DoubleQuoted : ScalarStyle.Any, true, false));
		}
	}
}
