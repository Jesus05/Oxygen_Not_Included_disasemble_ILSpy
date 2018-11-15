using Klei.AI;
using UnityEngine;

public class StoredMinionConfig : IEntityConfig
{
	public static string ID = "StoredMinion";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, true);
		gameObject.AddOrGet<SaveLoadRoot>();
		gameObject.AddOrGet<KPrefabID>().AddTag(ID);
		gameObject.AddOrGet<Equipment>();
		gameObject.AddOrGet<Ownables>();
		gameObject.AddOrGet<Traits>();
		gameObject.AddOrGet<Schedulable>();
		gameObject.AddOrGet<StoredMinionIdentity>();
		KSelectable kSelectable = gameObject.AddOrGet<KSelectable>();
		kSelectable.IsSelectable = false;
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
