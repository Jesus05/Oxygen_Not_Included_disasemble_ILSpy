namespace YamlDotNet.Core.Events
{
	public class SequenceEnd : ParsingEvent
	{
		public override int NestingIncrease => -1;

		internal override EventType Type => EventType.SequenceEnd;

		public SequenceEnd(Mark start, Mark end)
			: base(start, end)
		{
		}

		public SequenceEnd()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public override string ToString()
		{
			return "Sequence end";
		}

		public override void Accept(IParsingEventVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
