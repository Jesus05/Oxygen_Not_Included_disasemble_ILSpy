using TUNING;
using UnityEngine;

public class SolidTransferArmConfig : IBuildingConfig
{
	public const string ID = "SolidTransferArm";

	private const int RANGE = 4;

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolidTransferArm", 3, 1, "conveyor_transferarm_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
		buildingDef.Floodable = false;
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 120f;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 2f;
		buildingDef.PermittedRotations = PermittedRotations.R360;
		GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidTransferArm");
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<Operational>();
		go.AddOrGet<LoopingSounds>();
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
		AddVisualizer(go, true);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
		AddVisualizer(go, false);
		Constructable component = go.GetComponent<Constructable>();
		component.choreTags = GameTags.ChoreTypes.ConveyorChores;
		component.requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, LogicOperationalController.INPUT_PORTS_0_0);
		go.AddOrGet<LogicOperationalController>();
		SolidTransferArm solidTransferArm = go.AddOrGet<SolidTransferArm>();
		solidTransferArm.pickupRange = 4;
		AddVisualizer(go, false);
	}

	private static void AddVisualizer(GameObject prefab, bool movable)
	{
		StationaryChoreRangeVisualizer stationaryChoreRangeVisualizer = prefab.AddOrGet<StationaryChoreRangeVisualizer>();
		stationaryChoreRangeVisualizer.x = -4;
		stationaryChoreRangeVisualizer.y = -4;
		stationaryChoreRangeVisualizer.width = 9;
		stationaryChoreRangeVisualizer.height = 9;
		stationaryChoreRangeVisualizer.movable = movable;
	}
}
