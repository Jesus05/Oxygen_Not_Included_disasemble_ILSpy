using STRINGS;
using UnityEngine;

public class BabyDreckoPlasticConfig : IEntityConfig
{
	public const string ID = "DreckoPlasticBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = DreckoPlasticConfig.CreateDrecko("DreckoPlasticBaby", CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.BABY.NAME, CREATURES.SPECIES.DRECKO.VARIANT_PLASTIC.BABY.DESC, "baby_drecko_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "DreckoPlastic");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
