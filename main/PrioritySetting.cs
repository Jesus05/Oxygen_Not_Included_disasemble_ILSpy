using System;

public struct PrioritySetting : IComparable
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

	public int CompareTo(object obj)
	{
		if (!(obj is PrioritySetting))
		{
			return 1;
		}
		PriorityScreen.PriorityClass num = priority_class;
		PrioritySetting prioritySetting = (PrioritySetting)obj;
		if (num > prioritySetting.priority_class)
		{
			return 1;
		}
		PriorityScreen.PriorityClass num2 = priority_class;
		PrioritySetting prioritySetting2 = (PrioritySetting)obj;
		if (num2 < prioritySetting2.priority_class)
		{
			return -1;
		}
		int num3 = priority_value;
		PrioritySetting prioritySetting3 = (PrioritySetting)obj;
		if (num3 > prioritySetting3.priority_value)
		{
			return 1;
		}
		int num4 = priority_value;
		PrioritySetting prioritySetting4 = (PrioritySetting)obj;
		if (num4 < prioritySetting4.priority_value)
		{
			return -1;
		}
		return 0;
	}
}
