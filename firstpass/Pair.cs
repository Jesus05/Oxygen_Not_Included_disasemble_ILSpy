using System;

[Serializable]
public struct Pair<T, U> : IEquatable<Pair<T, U>>
{
	public T first;

	public U second;

	public Pair(T a, U b)
	{
		first = a;
		second = b;
	}

	public bool Equals(Pair<T, U> other)
	{
		return first.Equals(other.first) && second.Equals(other.second);
	}

	public override int GetHashCode()
	{
		return first.GetHashCode() ^ second.GetHashCode();
	}
}
