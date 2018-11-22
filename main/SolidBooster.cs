using UnityEngine;

public class SolidBooster : RocketEngine
{
	public Storage fuelStorage;

	[ContextMenu("Fill Tank")]
	public void FillTank()
	{
		Element element = ElementLoader.GetElement(fuelTag);
		GameObject go = element.substance.SpawnResource(base.gameObject.transform.GetPosition(), fuelStorage.capacityKg / 2f, element.defaultValues.temperature, byte.MaxValue, 0, false, false);
		fuelStorage.Store(go, false, false, true, false);
		element = ElementLoader.GetElement(GameTags.OxyRock);
		go = element.substance.SpawnResource(base.gameObject.transform.GetPosition(), fuelStorage.capacityKg / 2f, element.defaultValues.temperature, byte.MaxValue, 0, false, false);
		fuelStorage.Store(go, false, false, true, false);
	}
}
