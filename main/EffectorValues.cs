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
		if (obj is EffectorValues)
		{
			return Equals((EffectorValues)obj);
		}
		return false;
	}

	public bool Equals(EffectorValues p)
	{
		if (object.ReferenceEquals(p, null))
		{
			return false;
		}
		if (object.ReferenceEquals(this, p))
		{
			return true;
		}
		if (GetType() != p.GetType())
		{
			return false;
		}
		return amount == p.amount && radius == p.radius;
	}

	public override int GetHashCode()
	{
		return amount ^ radius;
	}

	public static bool operator ==(EffectorValues lhs, EffectorValues rhs)
	{
		if (object.ReferenceEquals(lhs, null))
		{
			if (object.ReferenceEquals(rhs, null))
			{
				return true;
			}
			return false;
		}
		return lhs.Equals(rhs);
	}

	public static bool operator !=(EffectorValues lhs, EffectorValues rhs)
	{
		return !(lhs == rhs);
	}
}
