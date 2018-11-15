public class PickupableSensor : Sensor
{
	private PathProber pathProber;

	private Worker worker;

	public PickupableSensor(Sensors sensors)
		: base(sensors)
	{
		worker = GetComponent<Worker>();
		pathProber = GetComponent<PathProber>();
	}

	public override void Update()
	{
		GlobalChoreProvider.Instance.UpdateFetches(pathProber);
		Game.Instance.fetchManager.UpdatePickups(pathProber, worker);
	}
}
