using STRINGS;
using UnityEngine;

public class BabyHatchConfig : IEntityConfig
{
	public const string ID = "HatchBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = HatchConfig.CreateHatch("HatchBaby", CREATURES.SPECIES.HATCH.BABY.NAME, CREATURES.SPECIES.HATCH.BABY.DESC, "baby_hatch_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Hatch");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
