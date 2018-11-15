using STRINGS;
using TUNING;
using UnityEngine;

public class PacuTropicalConfig : IEntityConfig
{
	public const string ID = "PacuTropical";

	public const string BASE_TRAIT_ID = "PacuTropicalBaseTrait";

	public const string EGG_ID = "PacuTropicalEgg";

	public static readonly EffectorValues DECOR = TUNING.BUILDINGS.DECOR.BONUS.TIER4;

	public const int EGG_SORT_ORDER = 502;

	public static GameObject CreatePacu(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BasePacuConfig.CreatePrefab(id, "PacuTropicalBaseTrait", name, desc, anim_file, is_baby, "trp_", 303.15f, 353.15f);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
		DecorProvider decorProvider = prefab.AddOrGet<DecorProvider>();
		decorProvider.SetValues(DECOR);
		return prefab;
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreatePacu("PacuTropical", STRINGS.CREATURES.SPECIES.PACU.VARIANT_TROPICAL.NAME, STRINGS.CREATURES.SPECIES.PACU.VARIANT_TROPICAL.DESC, "pacu_kanim", false);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "PacuTropicalEgg", STRINGS.CREATURES.SPECIES.PACU.VARIANT_TROPICAL.EGG_NAME, STRINGS.CREATURES.SPECIES.PACU.VARIANT_TROPICAL.DESC, "egg_pacu_kanim", PacuTuning.EGG_MASS, "PacuTropicalBaby", 15.000001f, 5f, PacuTuning.EGG_CHANCES_TROPICAL, 502, false, true, false, 0.75f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
