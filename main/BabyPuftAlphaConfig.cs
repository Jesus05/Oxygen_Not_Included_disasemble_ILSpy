using STRINGS;
using UnityEngine;

public class BabyPuftAlphaConfig : IEntityConfig
{
	public const string ID = "PuftAlphaBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = PuftAlphaConfig.CreatePuftAlpha("PuftAlphaBaby", CREATURES.SPECIES.PUFT.VARIANT_ALPHA.BABY.NAME, CREATURES.SPECIES.PUFT.VARIANT_ALPHA.BABY.DESC, "baby_puft_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "PuftAlpha");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BasePuftConfig.OnSpawn(inst);
	}
}
