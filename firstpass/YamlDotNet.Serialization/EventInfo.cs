namespace YamlDotNet.Serialization
{
	public abstract class EventInfo
	{
		public IObjectDescriptor Source
		{
			get;
			private set;
		}

		protected EventInfo(IObjectDescriptor source)
		{
			Source = source;
		}
	}
}
