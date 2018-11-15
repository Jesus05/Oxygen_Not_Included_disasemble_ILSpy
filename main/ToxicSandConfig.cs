using UnityEngine;

public class ToxicSandConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.ToxicSand;

	public SimHashes SublimeElementID => SimHashes.ContaminatedOxygen;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(ElementID, null);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
		sublimates.info = new Sublimates.Info(0.000200000009f, 0.00250000018f, 1.8f, 0.5f, SublimeElementID, byte.MaxValue, 0);
		return gameObject;
	}
}
