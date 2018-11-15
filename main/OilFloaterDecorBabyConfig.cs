using STRINGS;
using UnityEngine;

public class OilFloaterDecorBabyConfig : IEntityConfig
{
	public const string ID = "OilfloaterDecorBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = OilFloaterDecorConfig.CreateOilFloater("OilfloaterDecorBaby", CREATURES.SPECIES.OILFLOATER.VARIANT_DECOR.BABY.NAME, CREATURES.SPECIES.OILFLOATER.VARIANT_DECOR.BABY.DESC, "baby_oilfloater_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "OilfloaterDecor");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
