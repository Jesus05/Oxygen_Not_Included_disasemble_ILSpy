namespace YamlDotNet.Core.Events
{
	public class StreamEnd : ParsingEvent
	{
		public override int NestingIncrease => -1;

		internal override EventType Type => EventType.StreamEnd;

		public StreamEnd(Mark start, Mark end)
			: base(start, end)
		{
		}

		public StreamEnd()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public override string ToString()
		{
			return "Stream end";
		}

		public override void Accept(IParsingEventVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
