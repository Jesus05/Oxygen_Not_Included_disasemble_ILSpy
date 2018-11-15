using STRINGS;
using TUNING;
using UnityEngine;

public class MethaneGeyserConfig : IEntityConfig
{
	public const string ID = "MethaneGeyser";

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreatePlacedEntity("MethaneGeyser", STRINGS.CREATURES.SPECIES.METHANEGEYSER.NAME, STRINGS.CREATURES.SPECIES.METHANEGEYSER.DESC, 2000f, Assets.GetAnim("geyser_side_methane_kanim"), "inactive", Grid.SceneLayer.BuildingBack, 4, 2, TUNING.BUILDINGS.DECOR.BONUS.TIER1, NOISE_POLLUTION.NOISY.TIER5, SimHashes.Creature, null, 293f);
		gameObject.GetComponent<KPrefabID>().AddTag(GameTags.DeprecatedContent);
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.IgneousRock);
		component.Temperature = 372.15f;
		Geyser geyser = gameObject.AddOrGet<Geyser>();
		geyser.outputOffset = new Vector2I(0, 1);
		GeyserConfigurator geyserConfigurator = gameObject.AddOrGet<GeyserConfigurator>();
		geyserConfigurator.presetType = "methane";
		geyserConfigurator.presetMin = 0.35f;
		geyserConfigurator.presetMax = 0.65f;
		Studyable studyable = gameObject.AddOrGet<Studyable>();
		studyable.meterTrackerSymbol = "geotracker_target";
		studyable.meterAnim = "tracker";
		gameObject.AddOrGet<LoopingSounds>();
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
