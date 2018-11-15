using UnityEngine;

public class OxyRockConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.OxyRock;

	public SimHashes SublimeElementID => SimHashes.Oxygen;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(ElementID, null);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.OxygenEmissionBubbles;
		sublimates.info = new Sublimates.Info(0.0100000007f, 0.00500000035f, 1.8f, 0.7f, SublimeElementID, byte.MaxValue, 0);
		return gameObject;
	}
}
