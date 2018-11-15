public class Urge : Resource
{
	public Urge(string id)
		: base(id, null, null)
	{
	}

	public override string ToString()
	{
		return Id;
	}
}
