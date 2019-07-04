using System;

[Serializable]
public struct EffectorValues
{
	public int amount;

	public int radius;

	public EffectorValues(int amt, int rad)
	{
		amount = amt;
		radius = rad;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is EffectorValues))
		{
			return false;
		}
		return Equals((EffectorValues)obj);
	}

	public bool Equals(EffectorValues p)
	{
		if (!object.ReferenceEquals(p, null))
		{
			if (!object.ReferenceEquals(this, p))
			{
				if (GetType() == p.GetType())
				{
					return amount == p.amount && radius == p.radius;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return amount ^ radius;
	}

	public static bool operator ==(EffectorValues lhs, EffectorValues rhs)
	{
		if (!object.ReferenceEquals(lhs, null))
		{
			return lhs.Equals(rhs);
		}
		if (!object.ReferenceEquals(rhs, null))
		{
			return false;
		}
		return true;
	}

	public static bool operator !=(EffectorValues lhs, EffectorValues rhs)
	{
		return !(lhs == rhs);
	}
}
