namespace YamlDotNet.Serialization
{
	public class AliasEventInfo : EventInfo
	{
		public string Alias
		{
			get;
			set;
		}

		public AliasEventInfo(IObjectDescriptor source)
			: base(source)
		{
		}
	}
}
