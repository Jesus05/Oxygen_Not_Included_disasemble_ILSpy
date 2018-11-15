using System;
using System.Globalization;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.Converters
{
	public class DateTimeConverter : IYamlTypeConverter
	{
		private readonly DateTimeKind kind;

		private readonly IFormatProvider provider;

		private readonly string[] formats;

		public DateTimeConverter(DateTimeKind kind = DateTimeKind.Utc, IFormatProvider provider = null, params string[] formats)
		{
			this.kind = ((kind == DateTimeKind.Unspecified) ? DateTimeKind.Utc : kind);
			this.provider = (provider ?? CultureInfo.InvariantCulture);
			this.formats = formats.DefaultIfEmpty("G").ToArray();
		}

		public bool Accepts(Type type)
		{
			return type == typeof(DateTime);
		}

		public object ReadYaml(IParser parser, Type type)
		{
			string value = ((Scalar)parser.Current).Value;
			DateTimeStyles style = (kind != DateTimeKind.Local) ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal;
			DateTime dt = DateTime.ParseExact(value, formats, provider, style);
			dt = EnsureDateTimeKind(dt, kind);
			parser.MoveNext();
			return dt;
		}

		public void WriteYaml(IEmitter emitter, object value, Type type)
		{
			DateTime dateTime = (DateTime)value;
			string value2 = ((kind != DateTimeKind.Local) ? dateTime.ToUniversalTime() : dateTime.ToLocalTime()).ToString(formats.First(), provider);
			emitter.Emit(new Scalar(null, null, value2, ScalarStyle.Any, true, false));
		}

		private static DateTime EnsureDateTimeKind(DateTime dt, DateTimeKind kind)
		{
			if (dt.Kind == DateTimeKind.Local && kind == DateTimeKind.Utc)
			{
				return dt.ToUniversalTime();
			}
			if (dt.Kind == DateTimeKind.Utc && kind == DateTimeKind.Local)
			{
				return dt.ToLocalTime();
			}
			return dt;
		}
	}
}
