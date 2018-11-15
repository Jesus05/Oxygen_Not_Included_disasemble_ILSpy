namespace YamlDotNet.Core.Events
{
	public abstract class ParsingEvent
	{
		private readonly Mark start;

		private readonly Mark end;

		public virtual int NestingIncrease => 0;

		internal abstract EventType Type
		{
			get;
		}

		public Mark Start => start;

		public Mark End => end;

		internal ParsingEvent(Mark start, Mark end)
		{
			this.start = start;
			this.end = end;
		}

		public abstract void Accept(IParsingEventVisitor visitor);
	}
}
