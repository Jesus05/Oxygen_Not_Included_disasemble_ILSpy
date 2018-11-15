using UnityEngine;

public class BreathableAreaSensor : Sensor
{
	private bool isBreathable;

	private OxygenBreather breather;

	public BreathableAreaSensor(Sensors sensors)
		: base(sensors)
	{
	}

	public override void Update()
	{
		if ((Object)breather == (Object)null)
		{
			breather = GetComponent<OxygenBreather>();
		}
		bool flag = isBreathable;
		isBreathable = breather.IsBreathableElement;
		if (isBreathable != flag)
		{
			if (isBreathable)
			{
				Trigger(99949694, null);
			}
			else
			{
				Trigger(-1189351068, null);
			}
		}
	}

	public bool IsBreathable()
	{
		return isBreathable;
	}

	public bool IsUnderwater()
	{
		return breather.IsUnderLiquid;
	}
}
