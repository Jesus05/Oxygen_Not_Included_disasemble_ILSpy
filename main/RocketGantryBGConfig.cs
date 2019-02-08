using UnityEngine;

public class RocketGantryBGConfig : IEntityConfig
{
	public static readonly string ID = "RocketGantryBG";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, false);
		gameObject.AddOrGet<KBatchedAnimController>();
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
