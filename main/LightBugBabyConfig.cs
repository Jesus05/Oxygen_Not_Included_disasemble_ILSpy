using STRINGS;
using UnityEngine;

public class LightBugBabyConfig : IEntityConfig
{
	public const string ID = "LightBugBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = LightBugConfig.CreateLightBug("LightBugBaby", CREATURES.SPECIES.LIGHTBUG.BABY.NAME, CREATURES.SPECIES.LIGHTBUG.BABY.DESC, "baby_lightbug_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "LightBug", null);
		gameObject.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
