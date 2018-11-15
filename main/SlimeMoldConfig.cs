using UnityEngine;

public class SlimeMoldConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.SlimeMold;

	public SimHashes SublimeElementID => SimHashes.ContaminatedOxygen;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(ElementID, null);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
		sublimates.info = new Sublimates.Info(0.025f, 0.125f, 1.8f, 0f, SublimeElementID, byte.MaxValue, 0);
		return gameObject;
	}
}
