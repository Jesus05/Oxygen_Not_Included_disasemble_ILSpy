using STRINGS;
using UnityEngine;

public class BabyMoleConfig : IEntityConfig
{
	public const string ID = "MoleBaby";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = MoleConfig.CreateMole("MoleBaby", CREATURES.SPECIES.MOLE.BABY.NAME, CREATURES.SPECIES.MOLE.BABY.DESC, "baby_driller_kanim", true);
		EntityTemplates.ExtendEntityToBeingABaby(gameObject, "Mole");
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		MoleConfig.SetSpawnNavType(inst);
	}
}
