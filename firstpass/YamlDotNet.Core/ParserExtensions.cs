using System.Globalization;
using System.IO;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Core
{
	public static class ParserExtensions
	{
		public static T Expect<T>(this IParser parser) where T : ParsingEvent
		{
			T val = parser.Allow<T>();
			if (val == null)
			{
				ParsingEvent current = parser.Current;
				throw new YamlException(current.Start, current.End, string.Format(CultureInfo.InvariantCulture, "Expected '{0}', got '{1}' (at {2}).", typeof(T).Name, current.GetType().Name, current.Start));
			}
			return val;
		}

		public static bool Accept<T>(this IParser parser) where T : ParsingEvent
		{
			if (parser.Current == null && !parser.MoveNext())
			{
				throw new EndOfStreamException();
			}
			return parser.Current is T;
		}

		public static T Allow<T>(this IParser parser) where T : ParsingEvent
		{
			if (parser.Accept<T>())
			{
				T result = (T)parser.Current;
				parser.MoveNext();
				return result;
			}
			return (T)null;
		}

		public static T Peek<T>(this IParser parser) where T : ParsingEvent
		{
			if (parser.Accept<T>())
			{
				return (T)parser.Current;
			}
			return (T)null;
		}

		public static void SkipThisAndNestedEvents(this IParser parser)
		{
			int num = 0;
			do
			{
				num += parser.Peek<ParsingEvent>().NestingIncrease;
				parser.MoveNext();
			}
			while (num > 0);
		}
	}
}
