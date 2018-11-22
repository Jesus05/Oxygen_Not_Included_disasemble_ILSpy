using TUNING;
using UnityEngine;

public class SpecialCargoBayConfig : IBuildingConfig
{
	public const string ID = "SpecialCargoBay";

	public override BuildingDef CreateBuildingDef()
	{
		string id = "SpecialCargoBay";
		int width = 5;
		int height = 5;
		string anim = "rocket_storage_live_kanim";
		int hitpoints = 1000;
		float construction_time = 480f;
		float[] cARGO_MASS = BUILDINGS.ROCKETRY_MASS_KG.CARGO_MASS;
		string[] construction_materials = new string[1]
		{
			(-899253461).ToString()
		};
		float melting_point = 9999f;
		BuildLocationRule build_location_rule = BuildLocationRule.BuildingAttachPoint;
		EffectorValues tIER = NOISE_POLLUTION.NOISY.TIER2;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, cARGO_MASS, construction_materials, melting_point, build_location_rule, BUILDINGS.DECOR.NONE, tIER, 0.2f);
		BuildingTemplates.CreateRocketBuildingDef(buildingDef);
		buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
		buildingDef.OverheatTemperature = 2273.15f;
		buildingDef.Floodable = false;
		buildingDef.AttachmentSlotTag = GameTags.Rocket;
		buildingDef.ObjectLayer = ObjectLayer.Building;
		buildingDef.RequiresPowerInput = false;
		buildingDef.attachablePosition = new CellOffset(0, 0);
		buildingDef.CanMove = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
		go.AddOrGet<LoopingSounds>();
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		BuildingAttachPoint buildingAttachPoint = go.AddOrGet<BuildingAttachPoint>();
		buildingAttachPoint.points = new BuildingAttachPoint.HardPoint[1]
		{
			new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, null)
		};
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		CargoBay cargoBay = go.AddOrGet<CargoBay>();
		cargoBay.storage = go.AddOrGet<Storage>();
		cargoBay.storageType = CargoBay.CargoType.entities;
		cargoBay.storage.capacityKg = 100f;
		go.AddOrGet<RocketModule>();
		EntityTemplates.ExtendBuildingToRocketModule(go);
	}
}
