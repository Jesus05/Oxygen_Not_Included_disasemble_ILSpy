using UnityEngine;

public class RepairableStorageProxy : IEntityConfig
{
	public static string ID = "RepairableStorageProxy";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateEntity(ID, ID, true);
		gameObject.AddOrGet<Storage>();
		gameObject.AddTag(GameTags.NotConversationTopic);
		return gameObject;
	}

	public void OnPrefabInit(GameObject go)
	{
	}

	public void OnSpawn(GameObject go)
	{
	}
}
