using KSerialization;
using System;
using UnityEngine;

[Serializable]
[SerializationConfig(MemberSerialization.OptIn)]
public struct Tag : ISerializationCallbackReceiver, IEquatable<Tag>, IComparable<Tag>
{
	public static readonly Tag Invalid = default(Tag);

	[Serialize]
	[SerializeField]
	private string name;

	[Serialize]
	private int hash;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = string.Intern(value);
			hash = Hash.SDBMLower(name);
		}
	}

	public bool IsValid => hash != 0;

	public Tag(int hash)
	{
		this.hash = hash;
		name = "";
	}

	public Tag(Tag orig)
	{
		name = orig.name;
		hash = orig.hash;
	}

	public Tag(string name)
	{
		this.name = name;
		hash = Hash.SDBMLower(name);
	}

	public void Clear()
	{
		name = null;
		hash = 0;
	}

	public override int GetHashCode()
	{
		return hash;
	}

	public int GetHash()
	{
		return hash;
	}

	public override bool Equals(object obj)
	{
		Tag tag = (Tag)obj;
		return hash == tag.hash;
	}

	public bool Equals(Tag other)
	{
		return hash == other.hash;
	}

	public static bool operator ==(Tag a, Tag b)
	{
		return a.hash == b.hash;
	}

	public static bool operator !=(Tag a, Tag b)
	{
		return a.hash != b.hash;
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		if (name != null)
		{
			Name = name;
		}
		else
		{
			name = "";
		}
	}

	public int CompareTo(Tag other)
	{
		return hash - other.hash;
	}

	public override string ToString()
	{
		return (name == null) ? hash.ToString("X") : name;
	}

	public static implicit operator Tag(string s)
	{
		return new Tag(s);
	}
}
