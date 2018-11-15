using TUNING;
using UnityEngine;

public class ShearingStationConfig : IBuildingConfig
{
	public const string ID = "ShearingStation";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "ShearingStation";
		int width = 3;
		int height = 3;
		string anim = "shearing_station_kanim";
		int hitpoints = 100;
		float construction_time = 10f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
		string[] rAW_MINERALS = MATERIALS.RAW_MINERALS;
		float melting_point = 1600f;
		BuildLocationRule build_location_rule = BuildLocationRule.OnFloor;
		EffectorValues nONE = NOISE_POLLUTION.NONE;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, rAW_MINERALS, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, nONE, 0.2f);
		buildingDef.RequiresPowerInput = true;
		buildingDef.EnergyConsumptionWhenActive = 60f;
		buildingDef.ExhaustKilowattsWhenActive = 0.125f;
		buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
		buildingDef.Floodable = true;
		buildingDef.Entombable = true;
		buildingDef.AudioCategory = "Metal";
		buildingDef.AudioSize = "large";
		buildingDef.UtilityInputOffset = new CellOffset(0, 0);
		buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
		buildingDef.DefaultAnimState = "on";
		buildingDef.ShowInBuildMenu = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddOrGet<LoopingSounds>();
		go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RanchStation);
		RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
		roomTracker.requiredRoomType = Db.Get().RoomTypes.CreaturePen.Id;
		roomTracker.requirement = RoomTracker.Requirement.Required;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		RanchStation.Def def = go.AddOrGetDef<RanchStation.Def>();
		def.isCreatureEligibleToBeRanchedCb = ((GameObject creature_go, RanchStation.Instance ranch_station_smi) => creature_go.GetSMI<ScaleGrowthMonitor.Instance>()?.IsFullyGrown() ?? false);
		def.onRanchCompleteCb = delegate(GameObject creature_go)
		{
			creature_go.GetSMI<ScaleGrowthMonitor.Instance>().Shear();
		};
		def.interactLoopCount = 6;
		def.rancherInteractAnim = "anim_interacts_shearingstation_kanim";
		def.synchronizeBuilding = true;
		Prioritizable.AddRef(go);
	}
}
