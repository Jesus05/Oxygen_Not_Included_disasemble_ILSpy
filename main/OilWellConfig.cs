using STRINGS;
using TUNING;
using UnityEngine;

public class OilWellConfig : IEntityConfig
{
	public const string ID = "OilWell";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("OilWell", STRINGS.CREATURES.SPECIES.OIL_WELL.NAME, STRINGS.CREATURES.SPECIES.OIL_WELL.DESC, 2000f, Assets.GetAnim("geyser_side_oil_kanim"), "off", Grid.SceneLayer.BuildingBack, 4, 2, TUNING.BUILDINGS.DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER5, SimHashes.Creature, null, 293f);
		OccupyArea component = gameObject.GetComponent<OccupyArea>();
		component.objectLayers = new ObjectLayer[1]
		{
			ObjectLayer.Building
		};
		PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
		component2.SetElement(SimHashes.SedimentaryRock);
		component2.Temperature = 372.15f;
		BuildingAttachPoint buildingAttachPoint = gameObject.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 0), GameTags.OilWell, null)
		};
		SoundEventVolumeCache.instance.AddVolume("geyser_side_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
		SoundEventVolumeCache.instance.AddVolume("geyser_side_methane_kanim", "GeyserMethane_erupt_LP", NOISE_POLLUTION.NOISY.TIER6);
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}
}
