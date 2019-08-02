using STRINGS;
using TUNING;
using UnityEngine;

public class PacuCleanerConfig : IEntityConfig
{
	public const string ID = "PacuCleaner";

	public const string BASE_TRAIT_ID = "PacuCleanerBaseTrait";

	public const string EGG_ID = "PacuCleanerEgg";

	public const float POLLUTED_WATER_CONVERTED_PER_CYCLE = 120f;

	public const SimHashes INPUT_ELEMENT = SimHashes.DirtyWater;

	public const SimHashes OUTPUT_ELEMENT = SimHashes.Water;

	public static readonly EffectorValues DECOR = TUNING.BUILDINGS.DECOR.BONUS.TIER4;

	public const int EGG_SORT_ORDER = 501;

	public static GameObject CreatePacu(string id, string name, string desc, string anim_file, bool is_baby)
	{
		GameObject prefab = BasePacuConfig.CreatePrefab(id, "PacuCleanerBaseTrait", name, desc, anim_file, is_baby, "glp_", 243.15f, 278.15f);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
		if (!is_baby)
		{
			Storage storage = prefab.AddComponent<Storage>();
			storage.capacityKg = 10f;
			ElementConsumer elementConsumer = prefab.AddOrGet<PassiveElementConsumer>();
			elementConsumer.elementToConsume = SimHashes.DirtyWater;
			elementConsumer.consumptionRate = 0.2f;
			elementConsumer.capacityKG = 10f;
			elementConsumer.consumptionRadius = 3;
			elementConsumer.showInStatusPanel = true;
			elementConsumer.sampleCellOffset = new Vector3(0f, 0f, 0f);
			elementConsumer.isRequired = false;
			elementConsumer.storeOnConsume = true;
			elementConsumer.showDescriptor = false;
			prefab.AddOrGet<UpdateElementConsumerPosition>();
			BubbleSpawner bubbleSpawner = prefab.AddComponent<BubbleSpawner>();
			bubbleSpawner.element = SimHashes.Water;
			bubbleSpawner.emitMass = 2f;
			bubbleSpawner.emitVariance = 0.5f;
			bubbleSpawner.initialVelocity = new Vector2f(0, 1);
			ElementConverter elementConverter = prefab.AddOrGet<ElementConverter>();
			elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
			{
				new ElementConverter.ConsumedElement(SimHashes.DirtyWater.CreateTag(), 0.2f)
			};
			elementConverter.outputElements = new ElementConverter.OutputElement[1]
			{
				new ElementConverter.OutputElement(0.2f, SimHashes.Water, 0f, true, true, 0f, 0.5f, 1f, byte.MaxValue, 0)
			};
		}
		return prefab;
	}

	public GameObject CreatePrefab()
	{
		GameObject prefab = CreatePacu("PacuCleaner", STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.NAME, STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.DESC, "pacu_kanim", false);
		prefab = EntityTemplates.ExtendEntityToWildCreature(prefab, PacuTuning.PEN_SIZE_PER_CREATURE, 25f);
		return EntityTemplates.ExtendEntityToFertileCreature(prefab, "PacuCleanerEgg", STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.EGG_NAME, STRINGS.CREATURES.SPECIES.PACU.VARIANT_CLEANER.DESC, "egg_pacu_kanim", PacuTuning.EGG_MASS, "PacuCleanerBaby", 15.000001f, 5f, PacuTuning.EGG_CHANCES_CLEANER, 501, false, true, false, 0.75f);
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		ElementConsumer component = inst.GetComponent<ElementConsumer>();
		if ((Object)component != (Object)null)
		{
			component.EnableConsumption(true);
		}
	}
}
