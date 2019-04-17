using System.Diagnostics;

[DebuggerDisplay("{IdHash}")]
public class Resource
{
	public string Name;

	public string Id;

	public HashedString IdHash;

	public bool Disabled;

	public ResourceGuid Guid
	{
		get;
		private set;
	}

	public Resource()
	{
	}

	public Resource(string id, ResourceSet parent = null, string name = null)
	{
		Debug.Assert(id != null);
		Id = id;
		IdHash = new HashedString(Id);
		Guid = new ResourceGuid(id, parent);
		parent?.Add(this);
		if (name != null)
		{
			Name = name;
		}
		else
		{
			Name = id;
		}
	}

	public Resource(string id, string name)
	{
		Debug.Assert(id != null);
		Guid = new ResourceGuid(id, null);
		Id = id;
		IdHash = new HashedString(Id);
		Name = name;
	}

	public virtual void Initialize()
	{
	}
}
