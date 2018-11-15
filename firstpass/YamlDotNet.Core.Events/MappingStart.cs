using System.Globalization;

namespace YamlDotNet.Core.Events
{
	public class MappingStart : NodeEvent
	{
		private readonly bool isImplicit;

		private readonly MappingStyle style;

		public override int NestingIncrease => 1;

		internal override EventType Type => EventType.MappingStart;

		public bool IsImplicit => isImplicit;

		public override bool IsCanonical => !isImplicit;

		public MappingStyle Style => style;

		public MappingStart(string anchor, string tag, bool isImplicit, MappingStyle style, Mark start, Mark end)
			: base(anchor, tag, start, end)
		{
			this.isImplicit = isImplicit;
			this.style = style;
		}

		public MappingStart(string anchor, string tag, bool isImplicit, MappingStyle style)
			: this(anchor, tag, isImplicit, style, Mark.Empty, Mark.Empty)
		{
		}

		public MappingStart()
			: this(null, null, true, MappingStyle.Any, Mark.Empty, Mark.Empty)
		{
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Mapping start [anchor = {0}, tag = {1}, isImplicit = {2}, style = {3}]", base.Anchor, base.Tag, isImplicit, style);
		}

		public override void Accept(IParsingEventVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
