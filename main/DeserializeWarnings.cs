using UnityEngine;

public class DeserializeWarnings : KMonoBehaviour
{
	public struct Warning
	{
		private bool isSet;

		public void Warn(string message, GameObject obj = null)
		{
			if (!isSet)
			{
				Debug.LogWarning(message, obj);
				isSet = true;
			}
		}
	}

	public Warning BuildingTemeperatureIsZeroKelvin;

	public Warning PipeContentsTemperatureIsNan;

	public Warning PrimaryElementTemperatureIsNan;

	public Warning PrimaryElementHasNoElement;

	public static DeserializeWarnings Instance;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Instance = this;
	}
}
