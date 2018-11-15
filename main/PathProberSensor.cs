public class PathProberSensor : Sensor
{
	private Navigator navigator;

	public PathProberSensor(Sensors sensors)
		: base(sensors)
	{
		navigator = sensors.GetComponent<Navigator>();
	}

	public override void Update()
	{
		navigator.UpdateProbe();
	}
}
