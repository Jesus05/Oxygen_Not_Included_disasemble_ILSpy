using STRINGS;
using UnityEngine;

public class LightBugBlueBabyConfig : IEntityConfig
{
	public const string ID = "LightBugBlueBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = LightBugBlueConfig.CreateLightBug("LightBugBlueBaby", CREATURES.SPECIES.LIGHTBUG.VARIANT_BLUE.BABY.NAME, CREATURES.SPECIES.LIGHTBUG.VARIANT_BLUE.BABY.DESC, "baby_lightbug_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "LightBugBlue", null);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
