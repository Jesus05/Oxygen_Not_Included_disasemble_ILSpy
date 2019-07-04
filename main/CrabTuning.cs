using System.Collections.Generic;
using TUNING;

public static class CrabTuning
{
	public static List<FertilityMonitor.BreedingChance> EGG_CHANCES_BASE = new List<FertilityMonitor.BreedingChance>
	{
		new FertilityMonitor.BreedingChance
		{
			egg = "CrabEgg".ToTag(),
			weight = 0.98f
		}
	};

	public static float STANDARD_CALORIES_PER_CYCLE = 100000f;

	public static float STANDARD_STARVE_CYCLES = 10f;

	public static float STANDARD_STOMACH_SIZE = STANDARD_CALORIES_PER_CYCLE * STANDARD_STARVE_CYCLES;

	public static int PEN_SIZE_PER_CREATURE = CREATURES.SPACE_REQUIREMENTS.TIER3;

	public static float EGG_MASS = 2f;
}
