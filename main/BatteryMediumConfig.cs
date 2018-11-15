using TUNING;
using UnityEngine;

public class BatteryMediumConfig : BaseBatteryConfig
{
	public const string ID = "BatteryMedium";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef result = CreateBuildingDef("BatteryMedium", 2, 2, 30, "batterymed_kanim", 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.ALL_METALS, 800f, 0.25f, 1f, noise: NOISE_POLLUTION.NOISY.TIER1, decor: BUILDINGS.DECOR.PENALTY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("batterymed_kanim", "Battery_med_rattle", NOISE_POLLUTION.NOISY.TIER2);
		return result;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		Battery battery = go.AddOrGet<Battery>();
		battery.capacity = 40000f;
		battery.joulesLostPerSecond = battery.capacity * 0.05f / 600f;
		base.DoPostConfigureComplete(go);
	}
}
