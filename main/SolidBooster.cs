using UnityEngine;

public class SolidBooster : RocketEngine
{
	public Storage fuelStorage;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.gameObject.Subscribe(1366341636, OnReturn);
	}

	[ContextMenu("Fill Tank")]
	public void FillTank()
	{
		Element element = ElementLoader.GetElement(fuelTag);
		GameObject go = element.substance.SpawnResource(base.gameObject.transform.GetPosition(), fuelStorage.capacityKg / 2f, element.defaultValues.temperature, byte.MaxValue, 0, false, false, false);
		fuelStorage.Store(go, false, false, true, false);
		element = ElementLoader.GetElement(GameTags.OxyRock);
		go = element.substance.SpawnResource(base.gameObject.transform.GetPosition(), fuelStorage.capacityKg / 2f, element.defaultValues.temperature, byte.MaxValue, 0, false, false, false);
		fuelStorage.Store(go, false, false, true, false);
	}

	private void OnReturn(object data)
	{
		if ((Object)fuelStorage != (Object)null && fuelStorage.items != null)
		{
			for (int num = fuelStorage.items.Count - 1; num >= 0; num--)
			{
				Util.KDestroyGameObject(fuelStorage.items[num]);
			}
			fuelStorage.items.Clear();
		}
	}
}
