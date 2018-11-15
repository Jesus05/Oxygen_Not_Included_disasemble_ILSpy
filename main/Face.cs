public class Face : Resource
{
	public HashedString hash;

	public Face(string id)
		: base(id, null, null)
	{
		hash = new HashedString(id);
	}
}
