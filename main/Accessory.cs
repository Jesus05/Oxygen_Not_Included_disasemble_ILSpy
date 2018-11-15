public class Accessory : Resource
{
	public KAnim.Build.Symbol symbol
	{
		get;
		private set;
	}

	public HashedString batchSource
	{
		get;
		private set;
	}

	public AccessorySlot slot
	{
		get;
		private set;
	}

	public Accessory(string id, ResourceSet parent, AccessorySlot slot, HashedString batchSource, KAnim.Build.Symbol symbol)
		: base(id, parent, null)
	{
		this.slot = slot;
		this.symbol = symbol;
		this.batchSource = batchSource;
	}
}
