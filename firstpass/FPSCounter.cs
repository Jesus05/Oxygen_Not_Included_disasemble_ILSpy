using UnityEngine;

public class FPSCounter
{
	public static float FPSMeasurePeriod = 0.1f;

	private int FPSAccumulator;

	private float FPSNextPeriod;

	public static int currentFPS;

	private static FPSCounter instance;

	public static void Create()
	{
		if (instance == null)
		{
			instance = new FPSCounter();
			instance.FPSNextPeriod = Time.realtimeSinceStartup + FPSMeasurePeriod;
		}
	}

	public static void Update()
	{
		Create();
		instance.FPSAccumulator++;
		if (Time.realtimeSinceStartup > instance.FPSNextPeriod)
		{
			currentFPS = (int)((float)instance.FPSAccumulator / FPSMeasurePeriod);
			instance.FPSAccumulator = 0;
			instance.FPSNextPeriod += FPSMeasurePeriod;
		}
	}
}
