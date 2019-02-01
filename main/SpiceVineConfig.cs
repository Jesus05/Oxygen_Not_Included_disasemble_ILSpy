using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SpiceVineConfig : IEntityConfig
{
	public const string ID = "SpiceVine";

	public const string SEED_ID = "SpiceVineSeed";

	public const float FERTILIZATION_RATE = 0.00166666671f;

	public const float WATER_RATE = 0.0583333336f;

	public GameObject CreatePrefab()
	{
		string id = "SpiceVine";
		string name = STRINGS.CREATURES.SPECIES.SPICE_VINE.NAME;
		string desc = STRINGS.CREATURES.SPECIES.SPICE_VINE.DESC;
		float mass = 2f;
		KAnimFile anim = Assets.GetAnim("vinespicenut_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER1;
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.Hanging);
		list = list;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingFront, 1, 3, tIER, default(EffectorValues), SimHashes.Creature, list, 320f);
		EntityTemplates.MakeHangingOffsets(gameObject, 1, 3);
		GameObject template = gameObject;
		mass = 258.15f;
		float temperature_warning_low = 308.15f;
		float temperature_warning_high = 358.15f;
		float temperature_lethal_high = 448.15f;
		initialAnim = SpiceNutConfig.ID;
		EntityTemplates.ExtendEntityToBasicPlant(template, mass, temperature_warning_low, temperature_warning_high, temperature_lethal_high, null, true, 0f, 0.15f, initialAnim, true, true);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = tag,
				massConsumptionRate = 0.0583333336f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Phosphorite,
				massConsumptionRate = 0.00166666671f
			}
		});
		UprootedMonitor component = gameObject.GetComponent<UprootedMonitor>();
		component.monitorCell = new CellOffset(0, 1);
		gameObject.AddOrGet<StandardCropPlant>();
		template = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = "SpiceVineSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.SPICE_VINE.DESC;
		anim = Assets.GetAnim("seed_spicenut_kanim");
		list = new List<Tag>();
		list.Add(GameTags.CropSeed);
		list = list;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(template, productionType, initialAnim, desc, name, anim, "object", 1, list, SingleEntityReceptacle.ReceptacleDirection.Bottom, default(Tag), 4, STRINGS.CREATURES.SPECIES.SPICE_VINE.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false);
		GameObject template2 = EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "SpiceVine_preview", Assets.GetAnim("vinespicenut_kanim"), "place", 1, 3);
		EntityTemplates.MakeHangingOffsets(template2, 1, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
