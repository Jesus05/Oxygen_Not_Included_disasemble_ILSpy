using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class PuftBleachstoneConfig : IEntityConfig
{
	public const string ID = "PuftBleachstone";

	public const string BASE_TRAIT_ID = "PuftBleachstoneBaseTrait";

	public const string EGG_ID = "PuftBleachstoneEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.ChlorineGas;

	public const SimHashes EMIT_ELEMENT = SimHashes.BleachStone;

	private static float KG_ORE_EATEN_PER_CYCLE = 10f;

	private static float CALORIES_PER_KG_OF_ORE = PuftTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	private static float MIN_POOP_SIZE_IN_KG = 15f;

	public static int EGG_SORT_ORDER = PuftConfig.EGG_SORT_ORDER + 3;

	public static GameObject CreatePuftBleachstone(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BasePuftConfig.BasePuft(id, name, desc, "PuftBleachstoneBaseTrait", anim_file, is_baby, "anti_");
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, PuftTuning.PEN_SIZE_PER_CREATURE, 75f);
		Trait trait = Db.Get().CreateTrait("PuftBleachstoneBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PuftTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - PuftTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 75f, name, false, false, true));
		return BasePuftConfig.SetupDiet(prefab, SimHashes.ChlorineGas.CreateTag(), SimHashes.BleachStone.CreateTag(), CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0f, MIN_POOP_SIZE_IN_KG);
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreatePuftBleachstone("PuftBleachstone", STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.NAME, STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.DESC, "puft_kanim", false);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "PuftBleachstoneEgg", STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.EGG_NAME, STRINGS.CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.DESC, "egg_puft_kanim", PuftTuning.EGG_MASS, "PuftBleachstoneBaby", 45f, 15f, PuftTuning.EGG_CHANCES_BLEACHSTONE, EGG_SORT_ORDER, true, false, true, 1f);
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		BasePuftConfig.OnSpawn(inst);
	}
}
