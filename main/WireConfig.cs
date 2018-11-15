using TUNING;
using UnityEngine;

public class WireConfig : BaseWireConfig
{
	public const string ID = "Wire";

	public override BuildingDef CreateBuildingDef()
	{
		return CreateBuildingDef("Wire", "utilities_electric_kanim", 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, 0.05f, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.PENALTY.TIER0);
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		DoPostConfigureComplete(Wire.WattageRating.Max1000, go);
	}
}
