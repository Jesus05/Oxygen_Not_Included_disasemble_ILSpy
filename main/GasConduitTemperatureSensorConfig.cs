using TUNING;
using UnityEngine;

public class GasConduitTemperatureSensorConfig : ConduitSensorConfig
{
	public static string ID = "GasConduitTemperatureSensor";

	protected override ConduitType ConduitType => ConduitType.Gas;

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef(ID, "gas_temperature_sensor_kanim", BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, MATERIALS.REFINED_METALS);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		ConduitTemperatureSensor conduitTemperatureSensor = go.AddComponent<ConduitTemperatureSensor>();
		conduitTemperatureSensor.conduitType = ConduitType;
		conduitTemperatureSensor.Threshold = 280f;
		conduitTemperatureSensor.ActivateAboveThreshold = true;
		conduitTemperatureSensor.manuallyControlled = false;
		conduitTemperatureSensor.rangeMin = 0f;
		conduitTemperatureSensor.rangeMax = 9999f;
		conduitTemperatureSensor.defaultState = false;
	}
}
