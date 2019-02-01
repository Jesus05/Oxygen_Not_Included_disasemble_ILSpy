public static class UnitsUtil
{
	public static bool IsTimeUnit(Units unit)
	{
		if (unit != Units.PerSecond && unit != Units.PerDay)
		{
			return false;
		}
		return true;
	}

	public static string GetUnitSuffix(Units unit)
	{
		if (unit == Units.Kelvin)
		{
			return "K";
		}
		return "";
	}
}
