using TUNING;
using UnityEngine;

public class WireRefinedConfig : BaseWireConfig
{
	public const string ID = "WireRefined";

	public override BuildingDef CreateBuildingDef()
	{
		BuildingDef buildingDef = CreateBuildingDef("WireRefined", "utilities_electric_conduct_kanim", 3f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER0, 0.05f, noise: NOISE_POLLUTION.NONE, decor: BUILDINGS.DECOR.NONE);
		buildingDef.MaterialCategory = MATERIALS.REFINED_METALS;
		return buildingDef;
	}

	public override void DoPostConfigureComplete(GameObject go)
	{
		DoPostConfigureComplete(Wire.WattageRating.Max2000, go);
	}
}
