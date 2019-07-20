using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SaltPlantConfig : IEntityConfig
{
	public const string ID = "SaltPlant";

	public const string SEED_ID = "SaltPlantSeed";

	public const float FERTILIZATION_RATE = 0.0116666667f;

	public const float CHLORINE_CONSUMPTION_RATE = 0.006f;

	public GameObject CreatePrefab()
	{
		string id = "SaltPlant";
		string name = STRINGS.CREATURES.SPECIES.SALTPLANT.NAME;
		string desc = STRINGS.CREATURES.SPECIES.SALTPLANT.DESC;
		float mass = 2f;
		KAnimFile anim = Assets.GetAnim("saltplant_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.PENALTY.TIER1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.Hanging);
		list = list;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, list, 258.15f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 2);
		GameObject template = gameObject;
		mass = 198.15f;
		float temperature_warning_low = 248.15f;
		float temperature_warning_high = 323.15f;
		float temperature_lethal_high = 393.15f;
		initialAnim = 381665462.ToString();
		EntityTemplates.ExtendEntityToBasicPlant(template, mass, temperature_warning_low, temperature_warning_high, temperature_lethal_high, null, true, 0f, 0.15f, initialAnim, true, true, true, true, 2400f);
		gameObject.AddOrGet<SaltPlant>();
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = SimHashes.Sand.CreateTag(),
				massConsumptionRate = 0.0116666667f
			}
		});
		PressureVulnerable pressureVulnerable = gameObject.AddOrGet<PressureVulnerable>();
		PressureVulnerable pressureVulnerable2 = pressureVulnerable;
		temperature_lethal_high = 0.025f;
		temperature_warning_high = 0f;
		SimHashes[] safeAtmospheres = new SimHashes[1]
		{
			SimHashes.ChlorineGas
		};
		pressureVulnerable2.Configure(temperature_lethal_high, temperature_warning_high, 10f, 30f, safeAtmospheres);
		KPrefabID component = gameObject.GetComponent<KPrefabID>();
		component.prefabInitFn += delegate(GameObject inst)
		{
			PressureVulnerable component3 = inst.GetComponent<PressureVulnerable>();
			component3.safe_atmospheres.Add(ElementLoader.FindElementByHash(SimHashes.ChlorineGas));
		};
		Storage storage = gameObject.AddOrGet<Storage>();
		storage.showInUI = false;
		storage.capacityKg = 1f;
		ElementConsumer elementConsumer = gameObject.AddOrGet<ElementConsumer>();
		elementConsumer.showInStatusPanel = true;
		elementConsumer.showDescriptor = true;
		elementConsumer.storeOnConsume = false;
		elementConsumer.elementToConsume = SimHashes.ChlorineGas;
		elementConsumer.configuration = ElementConsumer.Configuration.Element;
		elementConsumer.consumptionRadius = 4;
		elementConsumer.sampleCellOffset = new Vector3(0f, -1f);
		elementConsumer.consumptionRate = 0.006f;
		UprootedMonitor component2 = gameObject.GetComponent<UprootedMonitor>();
		component2.monitorCell = new CellOffset(0, 1);
		gameObject.AddOrGet<StandardCropPlant>();
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = "SaltPlantSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.SALTPLANT.DESC;
		anim = Assets.GetAnim("seed_saltplant_kanim");
		list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Bottom, default(Tag), 4, STRINGS.CREATURES.SPECIES.SALTPLANT.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, null, "", false);
		GameObject template2 = EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "SaltPlant_preview", Assets.GetAnim("saltplant_kanim"), "place", 1, 2);
		EntityTemplates.MakeHangingOffsets(template2, 1, 2);
		return gameObject;
	}

	public void OnPrefabInit(GameObject prefab)
	{
	}

	public void OnSpawn(GameObject inst)
	{
		inst.GetComponent<ElementConsumer>().EnableConsumption(true);
	}
}
