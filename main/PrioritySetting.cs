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
		if (obj is PrioritySetting)
		{
			PrioritySetting prioritySetting = (PrioritySetting)obj;
			if (prioritySetting.priority_class == priority_class)
			{
				PrioritySetting prioritySetting2 = (PrioritySetting)obj;
				return prioritySetting2.priority_value == priority_value;
			}
			return false;
		}
		return false;
	}

	public int CompareTo(PrioritySetting other)
	{
		if (priority_class <= other.priority_class)
		{
			if (priority_class >= other.priority_class)
			{
				if (priority_value <= other.priority_value)
				{
					if (priority_value >= other.priority_value)
					{
						return 0;
					}
					return -1;
				}
				return 1;
			}
			return -1;
		}
		return 1;
	}
}
