using TUNING;
using UnityEngine;

public abstract class BaseBatteryConfig : IBuildingConfig
{
	public BuildingDef CreateBuildingDef(string id, int width, int height, int hitpoints, string anim, float construction_time, float[] construction_mass, string[] construction_materials, float melting_point, float exhaust_temperature_active, float self_heat_kilowatts_active, EffectorValues decor, EffectorValues noise)
	{
		EffectorValues tIER = NOISE_POLLUTION.NOISY.TIER0;
		BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, construction_mass, construction_materials, melting_point, BuildLocationRule.OnFloor, decor, tIER, 0.2f);
		buildingDef.ExhaustKilowattsWhenActive = exhaust_temperature_active;
		buildingDef.SelfHeatKilowattsWhenActive = self_heat_kilowatts_active;
		buildingDef.Entombable = false;
		buildingDef.ViewMode = OverlayModes.Power.ID;
		buildingDef.AudioCategory = "Metal";
		buildingDef.RequiresPowerOutput = true;
		buildingDef.UseWhitePowerOutputConnectorColour = true;
		return buildingDef;
	}

	public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
	{
		go.AddComponent<RequireInputs>();
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Battery battery = go.AddOrGet<Battery>();
		battery.powerSortOrder = 1000;
		go.AddOrGetDef<PoweredActiveController.Def>();
	}
}
