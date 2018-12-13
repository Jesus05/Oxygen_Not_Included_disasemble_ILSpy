using System;
using System.Diagnostics;
using UnityEngine;

[Serializable]
[DebuggerDisplay("Name = {DebuggerDisplay}")]
public struct KAnimHashedString : IComparable<KAnimHashedString>, IEquatable<KAnimHashedString>
{
	[SerializeField]
	private int hash;

	public int HashValue
	{
		get
		{
			return hash;
		}
		set
		{
			hash = value;
		}
	}

	public string DebuggerDisplay => HashCache.Get().Get(hash);

	public KAnimHashedString(string name)
	{
		hash = Hash.SDBMLower(name);
	}

	public KAnimHashedString(int hash)
	{
		this.hash = hash;
	}

	public bool IsValid()
	{
		return hash != 0;
	}

	public static implicit operator KAnimHashedString(HashedString hash)
	{
		return new KAnimHashedString(hash.HashValue);
	}

	public static implicit operator KAnimHashedString(string str)
	{
		return new KAnimHashedString(str);
	}

	public int CompareTo(KAnimHashedString obj)
	{
		if (hash < obj.hash)
		{
			return -1;
		}
		if (hash > obj.hash)
		{
			return 1;
		}
		return 0;
	}

	public override bool Equals(object obj)
	{
		KAnimHashedString kAnimHashedString = (KAnimHashedString)obj;
		return hash == kAnimHashedString.hash;
	}

	public bool Equals(KAnimHashedString other)
	{
		return hash == other.hash;
	}

	public override int GetHashCode()
	{
		return hash;
	}

	public static bool operator ==(KAnimHashedString x, HashedString y)
	{
		return x.HashValue == y.HashValue;
	}

	public static bool operator !=(KAnimHashedString x, HashedString y)
	{
		return x.HashValue != y.HashValue;
	}

	public static bool operator ==(KAnimHashedString x, KAnimHashedString y)
	{
		return x.hash == y.hash;
	}

	public static bool operator !=(KAnimHashedString x, KAnimHashedString y)
	{
		return x.hash != y.hash;
	}

	public override string ToString()
	{
		return string.IsNullOrEmpty(DebuggerDisplay) ? ("0x" + hash.ToString("X")) : DebuggerDisplay;
	}
}
