using System.Globalization;

namespace YamlDotNet.Core.Events
{
	public class DocumentEnd : ParsingEvent
	{
		private readonly bool isImplicit;

		public override int NestingIncrease => -1;

		internal override EventType Type => EventType.DocumentEnd;

		public bool IsImplicit => isImplicit;

		public DocumentEnd(bool isImplicit, Mark start, Mark end)
			: base(start, end)
		{
			this.isImplicit = isImplicit;
		}

		public DocumentEnd(bool isImplicit)
			: this(isImplicit, Mark.Empty, Mark.Empty)
		{
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "Document end [isImplicit = {0}]", isImplicit);
		}

		public override void Accept(IParsingEventVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
