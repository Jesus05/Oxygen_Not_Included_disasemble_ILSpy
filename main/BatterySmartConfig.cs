using STRINGS;
using TUNING;
using UnityEngine;

public class BatterySmartConfig : BaseBatteryConfig
{
	public const string ID = "BatterySmart";

	private static readonly LogicPorts.Port[] OUTPUT_PORTS = new LogicPorts.Port[1]
	{
		LogicPorts.Port.OutputPort(BatterySmart.PORT_ID, new CellOffset(0, 0), STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT, STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT_ACTIVE, STRINGS.BUILDINGS.PREFABS.BATTERYSMART.LOGIC_PORT_INACTIVE, true, false)
	};

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef result = CreateBuildingDef("BatterySmart", 2, 2, 30, "smartbattery_kanim", 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER3, MATERIALS.REFINED_METALS, 800f, 0f, 0.5f, noise: NOISE_POLLUTION.NOISY.TIER1, decor: TUNING.BUILDINGS.DECOR.PENALTY.TIER2);
		SoundEventVolumeCache.instance.AddVolume("batterymed_kanim", "Battery_med_rattle", NOISE_POLLUTION.NOISY.TIER2);
		return result;
	}

	public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, null, OUTPUT_PORTS);
	}

	public override void DoPostConfigureUnderConstruction(GameObject go)
	{
		GeneratedBuildings.RegisterLogicPorts(go, null, OUTPUT_PORTS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		BatterySmart batterySmart = go.AddOrGet<BatterySmart>();
		batterySmart.capacity = 20000f;
		batterySmart.joulesLostPerSecond = 0.6666667f;
		batterySmart.powerSortOrder = 1000;
		GeneratedBuildings.RegisterLogicPorts(go, null, OUTPUT_PORTS);
		base.DoPostConfigureComplete(go);
	}
}
