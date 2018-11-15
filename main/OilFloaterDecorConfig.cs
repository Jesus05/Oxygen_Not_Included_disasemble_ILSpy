using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

public class OilFloaterDecorConfig : IEntityConfig
{
	public const string ID = "OilfloaterDecor";

	public const string BASE_TRAIT_ID = "OilfloaterDecorBaseTrait";

	public const string EGG_ID = "OilfloaterDecorEgg";

	public const SimHashes CONSUME_ELEMENT = SimHashes.Oxygen;

	private static float KG_ORE_EATEN_PER_CYCLE = 30f;

	private static float CALORIES_PER_KG_OF_ORE = OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE / KG_ORE_EATEN_PER_CYCLE;

	public static int EGG_SORT_ORDER = OilFloaterConfig.EGG_SORT_ORDER + 2;

	public static GameObject CreateOilFloater(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject gameObject = BaseOilFloaterConfig.BaseOilFloater(id, name, desc, anim_file, "OilfloaterDecorBaseTrait", 283.15f, 343.15f, is_baby, "oxy_");
		DecorProvider decorProvider = gameObject.AddOrGet<DecorProvider>();
		decorProvider.SetValues(DECOR.BONUS.TIER6);
		EntityTemplates.ExtendEntityToWildCreature(gameObject, OilFloaterTuning.PEN_SIZE_PER_CREATURE, 150f);
		Trait trait = Db.Get().CreateTrait("OilfloaterDecorBaseTrait", name, name, null, false, null, true, true);
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, OilFloaterTuning.STANDARD_STOMACH_SIZE, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (0f - OilFloaterTuning.STANDARD_CALORIES_PER_CYCLE) / 600f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name, false, false, true));
		trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name, false, false, true));
		return BaseOilFloaterConfig.SetupDiet(gameObject, SimHashes.Oxygen.CreateTag(), Tag.Invalid, CALORIES_PER_KG_OF_ORE, 0f, null, 0f, 0f);
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = CreateOilFloater("OilfloaterDecor", STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_DECOR.NAME, STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_DECOR.DESC, "oilfloater_kanim", false);
		EntityTemplates.ExtendEntityToFertileCreature(gameObject, "OilfloaterDecorEgg", STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_DECOR.EGG_NAME, STRINGS.CREATURES.SPECIES.OILFLOATER.VARIANT_DECOR.DESC, "egg_oilfloater_kanim", OilFloaterTuning.EGG_MASS, "OilfloaterDecorBaby", 90f, 30f, OilFloaterTuning.EGG_CHANCES_DECOR, EGG_SORT_ORDER, true, false, true, 1f);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
