namespace YamlDotNet.Serialization
{
	public class ObjectEventInfo : EventInfo
	{
		public string Anchor
		{
			get;
			set;
		}

		public string Tag
		{
			get;
			set;
		}

		protected ObjectEventInfo(IObjectDescriptor source)
			: base(source)
		{
		}
	}
}
