using KSerialization;
using System.Runtime.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class ResourceRef<ResourceType> : ISaveLoadable where ResourceType : Resource
{
	[Serialize]
	private ResourceGuid guid;

	private ResourceType resource;

	public ResourceRef(ResourceType resource)
	{
		Set(resource);
	}

	public ResourceRef()
	{
	}

	public ResourceType Get()
	{
		return resource;
	}

	public void Set(ResourceType resource)
	{
		this.resource = resource;
	}

	[OnSerializing]
	private void OnSerializing()
	{
		if (resource == null)
		{
			guid = null;
		}
		else
		{
			guid = resource.Guid;
		}
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (guid != (ResourceGuid)null)
		{
			resource = Db.Get().GetResource<ResourceType>(guid);
			guid = null;
		}
	}
}
