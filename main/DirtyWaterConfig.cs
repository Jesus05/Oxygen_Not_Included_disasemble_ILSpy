using UnityEngine;

public class DirtyWaterConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.DirtyWater;

	public SimHashes SublimeElementID => SimHashes.ContaminatedOxygen;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLiquidOreEntity(ElementID, null);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
		sublimates.info = new Sublimates.Info(4.00000026E-05f, 0.025f, 1.8f, 1f, SublimeElementID, byte.MaxValue, 0);
		return gameObject;
	}
}
