namespace YamlDotNet.Core.Events
{
	public class MappingEnd : ParsingEvent
	{
		public override int NestingIncrease => -1;

		internal override EventType Type => EventType.MappingEnd;

		public MappingEnd(Mark start, Mark end)
			: base(start, end)
		{
		}

		public MappingEnd()
			: this(Mark.Empty, Mark.Empty)
		{
		}

		public override string ToString()
		{
			return "Mapping end";
		}

		public override void Accept(IParsingEventVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
