using KSerialization;
using System;

[SerializationConfig(MemberSerialization.OptIn)]
public class ResourceGuid : IEquatable<ResourceGuid>, ISaveLoadable
{
	[Serialize]
	public string Guid;

	public ResourceGuid(string id, Resource parent = null)
	{
		if (parent != null)
		{
			Guid = parent.Guid.Guid + "." + id;
		}
		else
		{
			Guid = id;
		}
	}

	public override int GetHashCode()
	{
		return Guid.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		ResourceGuid resourceGuid = (ResourceGuid)obj;
		if (obj != null)
		{
			return Guid == resourceGuid.Guid;
		}
		return false;
	}

	public bool Equals(ResourceGuid other)
	{
		return Guid == other.Guid;
	}

	public static bool operator ==(ResourceGuid a, ResourceGuid b)
	{
		if ((object)a == b)
		{
			return true;
		}
		if ((object)a == null)
		{
			return false;
		}
		if ((object)b == null)
		{
			return false;
		}
		return a.Guid == b.Guid;
	}

	public static bool operator !=(ResourceGuid a, ResourceGuid b)
	{
		if ((object)a == b)
		{
			return false;
		}
		if ((object)a == null)
		{
			return true;
		}
		if ((object)b == null)
		{
			return true;
		}
		return a.Guid != b.Guid;
	}

	public override string ToString()
	{
		return Guid;
	}
}
