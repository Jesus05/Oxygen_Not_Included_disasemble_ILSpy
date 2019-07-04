using STRINGS;
using UnityEngine;

public class OilFloaterHighTempBabyConfig : IEntityConfig
{
	public const string ID = "OilfloaterHighTempBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = OilFloaterHighTempConfig.CreateOilFloater("OilfloaterHighTempBaby", CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.BABY.NAME, CREATURES.SPECIES.OILFLOATER.VARIANT_HIGHTEMP.BABY.DESC, "baby_oilfloater_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "OilfloaterHighTemp", null);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
