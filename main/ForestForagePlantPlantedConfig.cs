using STRINGS;
using TUNING;
using UnityEngine;

public class ForestForagePlantPlantedConfig : IEntityConfig
{
	public const string ID = "ForestForagePlantPlanted";

	public GameObject CreatePrefab()
	{
		string id = "ForestForagePlantPlanted";
		string name = STRINGS.CREATURES.SPECIES.FORESTFORAGEPLANTPLANTED.NAME;
		string desc = STRINGS.CREATURES.SPECIES.FORESTFORAGEPLANTPLANTED.DESC;
		float mass = 100f;
		KAnimFile anim = Assets.GetAnim("podmelon_kanim");
		string initialAnim = "idle";
		EffectorValues tIER = DECOR.BONUS.TIER1;
		GameObject gameObject = EntityTemplates.CreatePlacedEntity(id, name, desc, mass, anim, initialAnim, Grid.SceneLayer.BuildingBack, 1, 2, tIER, default(EffectorValues), SimHashes.Creature, null, 293f);
		gameObject.AddOrGet<SimTemperatureTransfer>();
		OccupyArea occupyArea = gameObject.AddOrGet<OccupyArea>();
		occupyArea.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		gameObject.AddOrGet<EntombVulnerable>();
		gameObject.AddOrGet<DrowningMonitor>();
		gameObject.AddOrGet<Prioritizable>();
		gameObject.AddOrGet<Uprootable>();
		gameObject.AddOrGet<UprootedMonitor>();
		gameObject.AddOrGet<Harvestable>();
		gameObject.AddOrGet<HarvestDesignatable>();
		SeedProducer seedProducer = gameObject.AddOrGet<SeedProducer>();
		seedProducer.Configure("ForestForagePlant", SeedProducer.ProductionType.DigOnly, 1);
		gameObject.AddOrGet<BasicForagePlantPlanted>();
		gameObject.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
