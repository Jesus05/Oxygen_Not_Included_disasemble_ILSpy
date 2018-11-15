using TUNING;
using UnityEngine;

public class SolarPanelConfig : IBuildingConfig
{
	public const string ID = "SolarPanel";

	public const float WATTS_PER_LUX = 0.00053f;

	public const float MAX_WATTS = 380f;

	public override BuildingDef CreateBuildingDef()
	{
		string id = "SolarPanel";
		int width = 7;
		int height = 3;
		string anim = "solar_panel_kanim";
		int hitpoints = 100;
		float construction_time = 120f;
		float[] tIER = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
		string[] gLASSES = MATERIALS.GLASSES;
		float melting_point = 2400f;
		BuildLocationRule build_location_rule = BuildLocationRule.Anywhere;
		EffectorValues tIER2 = NOISE_POLLUTION.NOISY.TIER5;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tIER, gLASSES, melting_point, build_location_rule, BUILDINGS.DECOR.PENALTY.TIER2, tIER2, 0.2f);
		buildingDef.GeneratorWattageRating = 380f;
		buildingDef.GeneratorBaseCapacity = buildingDef.GeneratorWattageRating;
		buildingDef.ExhaustKilowattsWhenActive = 0f;
		buildingDef.SelfHeatKilowattsWhenActive = 0f;
		buildingDef.BuildLocationRule = BuildLocationRule.Anywhere;
		buildingDef.HitPoints = 10;
		buildingDef.ViewMode = SimViewMode.PowerMap;
		buildingDef.AudioCategory = "HollowMetal";
		buildingDef.AudioSize = "large";
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
		go.AddOrGet<LoopingSounds>();
		Prioritizable.AddRef(go);
		Tinkerable.MakePowerTinkerable(go);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Repairable repairable = go.AddOrGet<Repairable>();
		repairable.expectedRepairTime = 52.5f;
		SolarPanel solarPanel = go.AddOrGet<SolarPanel>();
		solarPanel.powerDistributionOrder = 9;
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
