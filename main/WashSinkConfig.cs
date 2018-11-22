using TUNING;
using UnityEngine;

public class WashSinkConfig : IBuildingConfig
{
	public const string ID = "WashSink";

	public const int DISEASE_REMOVAL_COUNT = 120000;

	public const float WATER_PER_USE = 5f;

	public const int USES_PER_FLUSH = 1;

	public const float WORK_TIME = 5f;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "WashSink";
		int width = 2;
		int height = 3;
		string anim = "wash_sink_kanim";
		int hitpoints = 30;
		float construction_time = 30f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] rAW_METALS = MATERIALS.RAW_METALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER1, tIER2, 0.2f);
		buildingDef.InputConduitType = ConduitType.Liquid;
		buildingDef.OutputConduitType = ConduitType.Liquid;
		buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(1, 1);
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.WashStation);
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.AdvancedWashStation);
		HandSanitizer handSanitizer = go.AddOrGet<HandSanitizer>();
		handSanitizer.massConsumedPerUse = 5f;
		handSanitizer.consumedElement = SimHashes.Water;
		handSanitizer.outputElement = SimHashes.DirtyWater;
		handSanitizer.diseaseRemovalCount = 120000;
		handSanitizer.maxUses = 1;
		handSanitizer.dirtyMeterOffset = Meter.Offset.Behind;
		go.AddOrGet<DirectionControl>();
		HandSanitizer.Work work = go.AddOrGet<HandSanitizer.Work>();
		work.overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_washbasin_kanim")
		};
		work.workTime = 5f;
		work.trackUses = true;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Liquid;
		conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
		conduitConsumer.capacityKG = 5f;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
		ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
		conduitDispenser.conduitType = ConduitType.Liquid;
		conduitDispenser.invertElementFilter = true;
		conduitDispenser.elementFilter = new SimHashes[1]
		{
			SimHashes.Water
		};
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 25f;
		storage.doDiseaseTransfer = false;
		storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
		go.AddOrGet<LoopingSounds>();
		RequireOutputs requireOutputs = go.AddOrGet<RequireOutputs>();
		requireOutputs.ignoreFullPipe = true;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
	}
}
