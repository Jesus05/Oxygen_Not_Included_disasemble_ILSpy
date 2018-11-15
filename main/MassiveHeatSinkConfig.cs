using TUNING;
using UnityEngine;

public class MassiveHeatSinkConfig : IBuildingConfig
{
	public const string ID = "MassiveHeatSink";

	private const float CONSUMPTION_RATE = 0.01f;

	private const float STORAGE_CAPACITY = 0.099999994f;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "MassiveHeatSink";
		int width = 4;
		int height = 4;
		string anim = "massiveheatsink_kanim";
		int hitpoints = 100;
		float construction_time = 120f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
		string[] rAW_METALS = MATERIALS.RAW_METALS;
		float melting_point = 2400f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_METALS, melting_point, build_location_rule, BUILDINGS.DECOR.BONUS.TIER2, tIER2, 0.2f);
		buildingDef.ExhaustKilowattsWhenActive = -16f;
		buildingDef.SelfHeatKilowattsWhenActive = -64f;
		buildingDef.Floodable = true;
		buildingDef.Entombable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.InputConduitType = ConduitType.Gas;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<MassiveHeatSink>();
		MinimumOperatingTemperature minimumOperatingTemperature = go.AddOrGet<MinimumOperatingTemperature>();
		minimumOperatingTemperature.minimumTemperature = 100f;
		PrimaryElement component = go.GetComponent<PrimaryElement>();
		component.SetElement(SimHashes.Iron);
		component.Temperature = 294.15f;
		go.AddOrGet<LoopingSounds>();
		Storage storage = go.AddOrGet<Storage>();
		storage.capacityKg = 0.099999994f;
		ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
		conduitConsumer.conduitType = ConduitType.Gas;
		conduitConsumer.consumptionRate = 1f;
		conduitConsumer.capacityTag = GameTagExtensions.Create(SimHashes.Hydrogen);
		conduitConsumer.capacityKG = 0.099999994f;
		conduitConsumer.forceAlwaysSatisfied = true;
		conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
		ElementConverter elementConverter = go.AddOrGet<ElementConverter>();
		elementConverter.consumedElements = new ElementConverter.ConsumedElement[1]
		{
			new ElementConverter.ConsumedElement(ElementLoader.FindElementByHash(SimHashes.Hydrogen).tag, 0.01f)
		};
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
