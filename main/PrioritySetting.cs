using System;

public struct PrioritySetting : IComparable<PrioritySetting>
{
	public PriorityScreen.PriorityClass priority_class;

	public int priority_value;

	public PrioritySetting(PriorityScreen.PriorityClass priority_class, int priority_value)
	{
		this.priority_class = priority_class;
		this.priority_value = priority_value;
	}

	public override int GetHashCode()
	{
		return ((int)priority_class << 28).GetHashCode() ^ priority_value.GetHashCode();
	}

	public static bool operator ==(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.Equals(rhs);
	}

	public static bool operator !=(PrioritySetting lhs, PrioritySetting rhs)
	{
		return !lhs.Equals(rhs);
	}

	public static bool operator <=(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.CompareTo(rhs) <= 0;
	}

	public static bool operator >=(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.CompareTo(rhs) >= 0;
	}

	public static bool operator <(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.CompareTo(rhs) < 0;
	}

	public static bool operator >(PrioritySetting lhs, PrioritySetting rhs)
	{
		return lhs.CompareTo(rhs) > 0;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is PrioritySetting))
		{
			return false;
		}
		PrioritySetting prioritySetting = (PrioritySetting)obj;
		if (prioritySetting.priority_class != priority_class)
		{
			return false;
		}
		PrioritySetting prioritySetting2 = (PrioritySetting)obj;
		return prioritySetting2.priority_value == priority_value;
	}

	public int CompareTo(PrioritySetting other)
	{
		if (priority_class > other.priority_class)
		{
			return 1;
		}
		if (priority_class < other.priority_class)
		{
			return -1;
		}
		if (priority_value > other.priority_value)
		{
			return 1;
		}
		if (priority_value < other.priority_value)
		{
			return -1;
		}
		return 0;
	}
}
