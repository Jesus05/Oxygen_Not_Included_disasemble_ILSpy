using System.Collections.Generic;

public class Sensors : KMonoBehaviour
{
	public List<Sensor> sensors = new List<Sensor>();

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<Brain>().onPreUpdate += OnBrainPreUpdate;
	}

	public SensorType GetSensor<SensorType>() where SensorType : Sensor
	{
		foreach (Sensor sensor in sensors)
		{
			if (typeof(SensorType).IsAssignableFrom(sensor.GetType()))
			{
				return (SensorType)sensor;
			}
		}
		Debug.LogError("Missing sensor of type: " + typeof(SensorType).Name, null);
		return (SensorType)null;
	}

	public void Add(Sensor sensor)
	{
		sensors.Add(sensor);
		sensor.Update();
	}

	public void UpdateSensors()
	{
		foreach (Sensor sensor in sensors)
		{
			sensor.Update();
		}
	}

	private void OnBrainPreUpdate()
	{
		UpdateSensors();
	}

	public void ShowEditor()
	{
		foreach (Sensor sensor in sensors)
		{
			sensor.ShowEditor();
		}
	}
}
