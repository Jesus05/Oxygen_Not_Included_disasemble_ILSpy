using UnityEngine;

public class Sensor
{
	protected Sensors sensors;

	public string Name
	{
		get;
		private set;
	}

	public GameObject gameObject => sensors.gameObject;

	public Transform transform => gameObject.transform;

	public Sensor(Sensors sensors)
	{
		this.sensors = sensors;
		Name = GetType().Name;
	}

	public ComponentType GetComponent<ComponentType>()
	{
		return sensors.GetComponent<ComponentType>();
	}

	public void Trigger(int hash, object data = null)
	{
		sensors.Trigger(hash, data);
	}

	public virtual void Update()
	{
	}

	public virtual void ShowEditor()
	{
	}
}
