using UnityEngine;

public class MeterConfig : IEntityConfig
{
	public static readonly string ID = "Meter";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, false);
		gameObject.AddOrGet<KBatchedAnimController>();
		gameObject.AddOrGet<KBatchedAnimTracker>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
