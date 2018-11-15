using STRINGS;
using UnityEngine;

public class BabyHatchMetalConfig : IEntityConfig
{
	public const string ID = "HatchMetalBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = HatchMetalConfig.CreateHatch("HatchMetalBaby", CREATURES.SPECIES.HATCH.VARIANT_METAL.BABY.NAME, CREATURES.SPECIES.HATCH.VARIANT_METAL.BABY.DESC, "baby_hatch_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "HatchMetal");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
