using TUNING;
using UnityEngine;

public class GasConduitDiseaseSensorConfig : ConduitSensorConfig
{
	public static string ID = "GasConduitDiseaseSensor";

	protected override ConduitType ConduitType => ConduitType.Gas;

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef(ID, "gas_germs_sensor_kanim", new float[2]
		{
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
			BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
		}, new string[2]
		{
			"RefinedMetal",
			"Plastic"
		});
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		base.DoPostConfigureComplete(go);
		ConduitDiseaseSensor conduitDiseaseSensor = go.AddComponent<ConduitDiseaseSensor>();
		conduitDiseaseSensor.conduitType = ConduitType;
		conduitDiseaseSensor.Threshold = 0f;
		conduitDiseaseSensor.ActivateAboveThreshold = true;
		conduitDiseaseSensor.manuallyControlled = false;
		conduitDiseaseSensor.defaultState = false;
	}
}
