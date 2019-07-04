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
		if (obj == null)
		{
			return false;
		}
		return Guid == resourceGuid.Guid;
	}

	public bool Equals(ResourceGuid other)
	{
		return Guid == other.Guid;
	}

	public static bool operator ==(ResourceGuid a, ResourceGuid b)
	{
		if ((object)a != b)
		{
			if ((object)a != null)
			{
				if ((object)b != null)
				{
					return a.Guid == b.Guid;
				}
				return false;
			}
			return false;
		}
		return true;
	}

	public static bool operator !=(ResourceGuid a, ResourceGuid b)
	{
		if ((object)a != b)
		{
			if ((object)a != null)
			{
				if ((object)b != null)
				{
					return a.Guid != b.Guid;
				}
				return true;
			}
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		return Guid;
	}
}
