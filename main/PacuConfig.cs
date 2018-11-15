using STRINGS;
using UnityEngine;

public class PacuConfig : IEntityConfig
{
	public const string ID = "Pacu";

	public const string BASE_TRAIT_ID = "PacuBaseTrait";

	public const string EGG_ID = "PacuEgg";

	public const int EGG_SORT_ORDER = 500;

	public static GameObject CreatePacu(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BasePacuConfig.CreatePrefab(id, "PacuBaseTrait", name, desc, anim_file, is_baby, null, 273.15f, 333.15f);
		return EntityTemplates.ExtendEntityToWildCreature(prefab, PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreatePacu("Pacu", CREATURES.SPECIES.PACU.NAME, CREATURES.SPECIES.PACU.DESC, "pacu_kanim", false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "PacuEgg", CREATURES.SPECIES.PACU.EGG_NAME, CREATURES.SPECIES.PACU.DESC, "egg_pacu_kanim", PacuTuning.EGG_MASS, "PacuBaby", 15.000001f, 5f, PacuTuning.EGG_CHANCES_BASE, 500, false, true, false, 0.75f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
		prefab.AddOrGet<LoopingSounds>();
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
