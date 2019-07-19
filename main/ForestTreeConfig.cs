using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ForestTreeConfig : IEntityConfig
{
	public const string ID = "ForestTree";

	public const string SEED_ID = "ForestTreeSeed";

	public const float FERTILIZATION_RATE = 0.0166666675f;

	public const float WATER_RATE = 0.116666667f;

	public const float BRANCH_GROWTH_TIME = 2100f;

	public const int NUM_BRANCHES = 7;

	public GameObject CreatePrefab()
	{
		string id = "ForestTree";
		string name = STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME;
		string desc = STRINGS.CREATURES.SPECIES.WOOD_TREE.DESC;
		float mass = 2f;
		KAnimFile anim = Assets.GetAnim("tree_kanim");
		string initialAnim = "idle_empty";
		EffectorValues tIER = DECOR.BONUS.TIER1;
		List<Tag> additionalTags = new List<Tag>();
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.Building, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, additionalTags, 298.15f);
		EntityTemplates.ExtendEntityToBasicPlant(gameObject, 258.15f, 288.15f, 313.15f, 448.15f, null, true, 0f, 0.15f, "WoodLog", true, true, true, false, 2400f);
		gameObject.AddOrGet<BuddingTrunk>();
		gameObject.UpdateComponentRequirement<Harvestable>(false);
		Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
		EntityTemplates.ExtendPlantToIrrigated(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = tag,
				massConsumptionRate = 0.116666667f
			}
		});
		EntityTemplates.ExtendPlantToFertilizable(gameObject, new PlantElementAbsorber.ConsumeInfo[1]
		{
			new PlantElementAbsorber.ConsumeInfo
			{
				tag = GameTags.Dirt,
				massConsumptionRate = 0.0166666675f
			}
		});
		gameObject.AddComponent<StandardCropPlant>();
		UprootedMonitor component = gameObject.GetComponent<UprootedMonitor>();
		component.monitorCell = new CellOffset(0, -1);
		BuddingTrunk buddingTrunk = gameObject.AddOrGet<BuddingTrunk>();
		buddingTrunk.budPrefabID = "ForestTreeBranch";
		GameObject plant = gameObject;
		SeedProducer.ProductionType productionType = SeedProducer.ProductionType.Harvest;
		initialAnim = "ForestTreeSeed";
		desc = STRINGS.CREATURES.SPECIES.SEEDS.WOOD_TREE.NAME;
		name = STRINGS.CREATURES.SPECIES.SEEDS.WOOD_TREE.DESC;
		anim = Assets.GetAnim("seed_tree_kanim");
		additionalTags = new List<Tag>();
		additionalTags.Add(GameTags.CropSeed);
		additionalTags = additionalTags;
		GameObject seed = EntityTemplates.CreateAndRegisterSeedForPlant(plant, productionType, initialAnim, desc, name, anim, "object", 1, additionalTags, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, STRINGS.CREATURES.SPECIES.WOOD_TREE.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false);
		EntityTemplates.CreateAndRegisterPreviewForPlant(seed, "ForestTree_preview", Assets.GetAnim("tree_kanim"), "place", 3, 3);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
