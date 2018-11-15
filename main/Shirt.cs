public class Shirt : Resource
{
	public HashedString hash;

	public Shirt(string id)
		: base(id, null, null)
	{
		hash = new HashedString(id);
	}
}
