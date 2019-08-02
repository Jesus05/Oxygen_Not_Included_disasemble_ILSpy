using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftConfig : IEntityConfig
{
	public const string ID = "Puft";

	public const string BASE_TRAIT_ID = "PuftBaseTrait";

	public const string EGG_ID = "PuftEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.ContaminatedOxygen;

	public const SimHashes EMIT_ELEMENT = SimHashes.SlimeMold;

	public const string EMIT_DISEASE = "SlimeLung";

	public const float EMIT_DISEASE_PER_KG = 1000f;

	private static float KG_ORE_EATEN_PER_CYCLE = 50f;

	private static float CALORIES_PER_KG_OF_ORE = PuftTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 15f;

	public static int EGG_SORT_ORDER = 300;

	public static GameObject CreatePuft(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BasePuftConfig.BasePuft(id, name, STRINGS.CREATURES.SPECIES.PUFT.DESC, "PuftBaseTrait", anim_file, is_baby, null, 288.15f, 328.15f);
		EntityTemplates.ExtendEntityToWildCreature(prefab, PuftTuning.PEN_SIZE_PER_CREATURE, 75f);
		Trait trait = Db.Get().CreateTrait("PuftBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - PuftTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name, false, false, true));
		prefab = BasePuftConfig.SetupDiet(prefab, SimHashes.ContaminatedOxygen.CreateTag(), SimHashes.SlimeMold.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_2, "SlimeLung", 1000f, MIN_POOP_SIZE_IN_KG);
		DiseaseSourceVisualizer diseaseSourceVisualizer = prefab.AddOrGet<DiseaseSourceVisualizer>();
		diseaseSourceVisualizer.alwaysShowDisease = "SlimeLung";
		return prefab;
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreatePuft("Puft", STRINGS.CREATURES.SPECIES.PUFT.NAME, STRINGS.CREATURES.SPECIES.PUFT.DESC, "puft_kanim", false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "PuftEgg", STRINGS.CREATURES.SPECIES.PUFT.EGG_NAME, STRINGS.CREATURES.SPECIES.PUFT.DESC, "egg_puft_kanim", PuftTuning.EGG_MASS, "PuftBaby", 45f, 15f, PuftTuning.EGG_CHANCES_BASE, EGG_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BasePuftConfig.OnSpawn(inst);
	}
}
