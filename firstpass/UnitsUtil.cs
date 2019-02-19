public static class UnitsUtil
{
	public static bool IsTimeUnit(Units unit)
	{
		if (unit == Units.PerSecond || unit == Units.PerDay)
		{
			return true;
		}
		return false;
	}

	public static string GetUnitSuffix(Units unit)
	{
		if (unit == Units.Kelvin)
		{
			return "K";
		}
		return string.Empty;
	}
}
