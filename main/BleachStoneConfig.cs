using UnityEngine;

public class BleachStoneConfig : IOreConfig
{
	public SimHashes ElementID => SimHashes.BleachStone;

	public SimHashes SublimeElementID => SimHashes.ChlorineGas;

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateSolidOreEntity(ElementID, null);
		Sublimates sublimates = gameObject.AddOrGet<Sublimates>();
		sublimates.spawnFXHash = SpawnFXHashes.BleachStoneEmissionBubbles;
		sublimates.info = new Sublimates.Info(0.000200000009f, 0.00250000018f, 1.8f, 0.5f, SublimeElementID, byte.MaxValue, 0);
		return gameObject;
	}
}
